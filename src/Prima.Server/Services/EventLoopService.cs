using System.Collections.Concurrent;
using System.Diagnostics;
using Orion.Core.Server.Events.Diagnostic;
using Orion.Core.Server.Interfaces.Metrics;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Config.Internal.EventLoop;
using Prima.Core.Server.Data.Metrics.EventLoop;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Server.Services;

/// <summary>
/// EventLoop service implementation inspired by Ultima Online emulators.
/// Manages action execution based on priorities and in FIFO order within
/// each priority level.
/// </summary>
public class EventLoopService : IEventLoopService, IDisposable, IMetricsProvider
{
    private readonly ILogger<EventLoopService> _logger;
    private readonly IEventBusService _eventBusService;
    private readonly EventLoopConfig _config;

    public object GetMetrics()
    {
        return Metrics;
    }


    public string ProviderName => "EventLoopService";

    // Dictionary with priorities as keys and FIFO queues as values
    private readonly ConcurrentDictionary<EventLoopPriority, ConcurrentQueue<QueuedAction>> _priorityQueues = new();

    // Tracks actions by ID to allow cancellation
    private readonly ConcurrentDictionary<Guid, QueuedActionReference> _actionRegistry = new();

    // Actions scheduled for future execution
    private readonly ConcurrentDictionary<Guid, DelayedAction> _delayedActions = new();

    private readonly object _tickLock = new();
    private CancellationTokenSource _cancellationTokenSource;
    private Task _loopTask;
    private long _lastTickTimestamp;
    private bool _isRunning;
    private bool _isDisposed;

    // Performance metrics
    public EventLoopMetrics Metrics { get; } = new EventLoopMetrics();

    // Events
    public event IEventLoopService.EventLoopTickHandler OnTick;
    public event IEventLoopService.EventLoopResetHandler OnTickReset;

    /// <summary>
    /// Gets or sets the interval in milliseconds between each tick of the event loop.
    /// </summary>
    public int TickIntervalMs
    {
        get => _config.TickIntervalMs;
        set => _config.TickIntervalMs = value;
    }

    /// <summary>
    /// Initializes a new instance of the EventLoopService class.
    /// </summary>
    public EventLoopService(ILogger<EventLoopService> logger, IEventBusService eventBusService, EventLoopConfig config)
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _config = config;

        // Initialize queues for each priority level
        foreach (EventLoopPriority priority in Enum.GetValues(typeof(EventLoopPriority)))
        {
            _priorityQueues[priority] = new ConcurrentQueue<QueuedAction>();
        }
    }

    /// <summary>
    /// Starts the event loop service.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
            return;

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _loopTask = Task.Run(EventLoopAsync, _cancellationTokenSource.Token);

        _logger.LogInformation("EventLoopService started with tick interval of {TickIntervalMs}ms", TickIntervalMs);
    }

    /// <summary>
    /// The main event loop that runs on a dedicated task.
    /// </summary>
    private async Task EventLoopAsync()
    {
        try
        {
            _logger.LogDebug("Event loop task started");

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                long tickStartTimestamp = Stopwatch.GetTimestamp();

                // Process a single tick
                ProcessTick();

                // Calculate actual execution time for this tick
                long tickEndTimestamp = Stopwatch.GetTimestamp();
                double tickDurationMs = Stopwatch.GetElapsedTime(tickStartTimestamp, tickEndTimestamp).TotalMilliseconds;

                // Determine delay to maintain consistent tick rate
                int delayMs = tickDurationMs >= TickIntervalMs
                    ? 1 // Minimal delay to allow other tasks to run
                    : (int)(TickIntervalMs - tickDurationMs);

                // Wait until the next tick
                await Task.Delay(delayMs, _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when token is cancelled, no need to log as error
            _logger.LogDebug("Event loop task cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in event loop task");
        }
        finally
        {
            _logger.LogDebug("Event loop task stopped");
        }
    }

    /// <summary>
    /// Stops the event loop service.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!_isRunning)
            return;

        _isRunning = false;

        if (_cancellationTokenSource != null)
        {
            _logger.LogInformation("Stopping event loop service...");
            await _cancellationTokenSource.CancelAsync();

            try
            {
                // Wait for the loop task to complete (with timeout)
                if (_loopTask != null)
                {
                    var timeoutTask = Task.Delay(5000, cancellationToken); // 5 second timeout
                    await Task.WhenAny(_loopTask, timeoutTask);

                    if (!_loopTask.IsCompleted)
                    {
                        _logger.LogWarning("Event loop task did not complete within timeout period");
                    }
                    else
                    {
                        _logger.LogInformation("Event loop task completed successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping event loop service");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _loopTask = null;
            }
        }
    }

    /// <summary>
    /// Enqueues an action to be executed with normal priority.
    /// </summary>
    public Guid EnqueueAction(string name, Action action)
    {
        return EnqueueAction(name, action, EventLoopPriority.Normal);
    }

    /// <summary>
    /// Enqueues an action to be executed with the specified priority.
    /// </summary>
    public Guid EnqueueAction(string name, Action action, EventLoopPriority priority)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var queuedAction = new QueuedAction(name, action, priority);
        var queue = _priorityQueues[priority];
        queue.Enqueue(queuedAction);

        // Register the action to allow cancellation later
        _actionRegistry[queuedAction.Id] = new QueuedActionReference(priority, queuedAction.Id);

        UpdatePriorityMetrics();

        _logger.LogTrace(
            "Action '{Name}' with ID {Id} enqueued with priority {Priority}",
            name,
            queuedAction.Id,
            priority
        );

        return queuedAction.Id;
    }

    /// <summary>
    /// Enqueues an action to be executed after the specified delay with normal priority.
    /// </summary>
    public Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay)
    {
        return EnqueueDelayedAction(name, action, delay, EventLoopPriority.Normal);
    }

    /// <summary>
    /// Enqueues an action to be executed after the specified delay with the specified priority.
    /// </summary>
    public Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay, EventLoopPriority priority)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var queuedAction = new QueuedAction(name, action, priority);
        var executeAt = DateTime.UtcNow.Add(delay);
        var delayedAction = new DelayedAction(queuedAction, executeAt);

        _delayedActions[queuedAction.Id] = delayedAction;

        _logger.LogTrace(
            "Delayed action '{Name}' with ID {Id} enqueued with priority {Priority} to execute at {ExecuteAt}",
            name,
            queuedAction.Id,
            priority,
            executeAt
        );

        return queuedAction.Id;
    }

    /// <summary>
    /// Tries to cancel a previously enqueued action.
    /// </summary>
    public bool TryCancelAction(Guid actionId)
    {
        // First check in delayed actions
        if (_delayedActions.TryRemove(actionId, out var delayedAction))
        {
            _logger.LogTrace(
                "Delayed action '{Name}' with ID {Id} was cancelled",
                delayedAction.Action.Name,
                actionId
            );

            UpdatePriorityMetrics();
            return true;
        }

        // Then check in regular actions
        if (_actionRegistry.TryRemove(actionId, out var reference))
        {
            // We can't directly remove from a ConcurrentQueue, so we mark it as cancelled
            // and will skip it when processing
            _logger.LogTrace(
                "Action '{0}' with ID {1} was marked for cancellation",
                reference.Priority,
                actionId
            );

            UpdatePriorityMetrics();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Processes a single tick of the event loop.
    /// </summary>
    private void ProcessTick()
    {
        // Use a lock to prevent concurrent ticks
        if (!Monitor.TryEnter(_tickLock))
        {
            return;
        }

        try
        {
            long tickStartTimestamp = Stopwatch.GetTimestamp();

            if (_lastTickTimestamp != 0)
            {
                double timeSinceLastTick =
                    Stopwatch.GetElapsedTime(_lastTickTimestamp, tickStartTimestamp).TotalMilliseconds;

                if (Math.Abs(timeSinceLastTick - TickIntervalMs) > TickIntervalMs * 0.5)
                {
                    _logger.LogWarning(
                        "Irregular tick detected: {TimeSinceLastTick}ms since last tick (expected {TickIntervalMs}ms)",
                        timeSinceLastTick.ToString("F2"),
                        TickIntervalMs
                    );
                }

                Metrics.AverageTimeBetweenTicksMs =
                    (Metrics.AverageTimeBetweenTicksMs * 0.9) + (timeSinceLastTick * 0.1);
            }

            // Process delayed actions that are due
            ProcessDelayedActions();

            // Process queued actions by priority
            ProcessQueuedActions();

            // Calculate tick duration
            long tickEndTimestamp = Stopwatch.GetTimestamp();
            double tickDurationMs = Stopwatch.GetElapsedTime(tickStartTimestamp, tickEndTimestamp).TotalMilliseconds;

            // Increment tick counter and accumulate time
            Metrics.TotalTicksProcessed++;
            Metrics.AccumulatedTickTimeMs += tickDurationMs;

            OnTick?.Invoke(tickDurationMs);

            // Reset counters periodically
            if (Metrics.TotalTicksProcessed % 1000 == 0)
            {
                _logger.LogInformation(
                    "Event loop stats: Processed {TotalTicks} ticks, Avg tick time: {AvgTickTime}ms, Last tick: {LastTickTime}ms, Queue size: {QueueSize}",
                    Metrics.TotalTicksProcessed,
                    (Metrics.AccumulatedTickTimeMs / 1000).ToString("F2"),
                    tickDurationMs.ToString("F2"),
                    Metrics.QueuedActionsCount
                );

                // Reset accumulated time for the next 1000 ticks
                Metrics.AccumulatedTickTimeMs = 0;

                OnTickReset?.Invoke();
            }

            // Update metrics
            Metrics.LastTickTimeMs = tickDurationMs;
            Metrics.QueuedActionsCount = GetTotalQueuedActionsCount();

            // Log action processing stats periodically
            if (Metrics.TotalActionsProcessed % 1000 == 0 && Metrics.TotalActionsProcessed > 0)
            {
                _logger.LogDebug(
                    "Action processing stats: Processed {TotalActions} actions, Avg time: {AvgTime}ms, Max time: {MaxTime}ms",
                    Metrics.TotalActionsProcessed,
                    Metrics.AverageProcessingTimeMs.ToString("F2"),
                    Metrics.MaxProcessingTimeMs.ToString("F2")
                );
            }

            _lastTickTimestamp = Stopwatch.GetTimestamp();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in event loop tick");
        }
        finally
        {
            Monitor.Exit(_tickLock);
        }
    }

    /// <summary>
    /// Processes delayed actions that are due to be executed.
    /// </summary>
    private void ProcessDelayedActions()
    {
        var now = DateTime.UtcNow;
        var actionsToExecute = _delayedActions
            .Where(kv => kv.Value.ExecuteAt <= now)
            .ToList();

        foreach (var actionEntry in actionsToExecute)
        {
            if (_delayedActions.TryRemove(actionEntry.Key, out var delayedAction))
            {
                var priority = delayedAction.Action.Priority;
                _priorityQueues[priority].Enqueue(delayedAction.Action);
                _actionRegistry[delayedAction.Action.Id] = new QueuedActionReference(priority, delayedAction.Action.Id);

                _logger.LogTrace(
                    "Delayed action '{Name}' with ID {Id} is now ready for execution",
                    delayedAction.Action.Name,
                    actionEntry.Key
                );
            }
        }

        UpdatePriorityMetrics();
    }

    /// <summary>
    /// Processes queued actions according to their priority.
    /// </summary>
    private void ProcessQueuedActions()
    {
        var totalQueuedActions = GetTotalQueuedActionsCount();
        if (totalQueuedActions == 0)
            return;

        // Process queues in descending priority order (highest first)
        var prioritiesInOrder = _priorityQueues.Keys
            .OrderByDescending(p => p)
            .ToList();

        var actionsProcessed = 0;
        var totalProcessingTime = 0.0;
        var maxProcessingTime = 0.0;
        var maxActionsToProcess = _config.MaxActionsPerTick;

        // For each priority level, process actions in FIFO order
        foreach (var priority in prioritiesInOrder)
        {
            var queue = _priorityQueues[priority];

            while (queue.TryDequeue(out var action))
            {
                if (!_isRunning || actionsProcessed >= maxActionsToProcess)
                    return;

                // Check if the action was cancelled
                if (!_actionRegistry.ContainsKey(action.Id))
                {
                    // Action was cancelled, skip it
                    continue;
                }

                // Remove the action from the registry
                _actionRegistry.TryRemove(action.Id, out _);

                try
                {
                    _logger.LogTrace("Executing action '{Name}' with ID {Id}", action.Name, action.Id);

                    // Record start timestamp
                    var executionStartTimestamp = Stopwatch.GetTimestamp();

                    // Execute the action
                    action.Action.Invoke();

                    // Record end timestamp
                    var executionEndTimestamp = Stopwatch.GetTimestamp();

                    // Calculate execution time
                    var processingTime = Stopwatch.GetElapsedTime(
                            executionStartTimestamp,
                            executionEndTimestamp
                        )
                        .TotalMilliseconds;
                    totalProcessingTime += processingTime;
                    maxProcessingTime = Math.Max(maxProcessingTime, processingTime);
                    actionsProcessed++;

                    // Log slow actions
                    if (processingTime > _config.SlowActionThresholdMs)
                    {
                        _logger.LogWarning(
                            "Slow action detected: '{Name}' ({ActionId}) took {ProcessingTime}ms to execute with priority {Priority}",
                            action.Name,
                            action.Id,
                            processingTime.ToString("F2"),
                            action.Priority
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error executing action '{Name}' ({ActionId}) with priority {Priority}",
                        action.Name,
                        action.Id,
                        action.Priority
                    );
                }
            }
        }

        // Update metrics
        if (actionsProcessed > 0)
        {
            Metrics.ActionsProcessedInTick = actionsProcessed;
            Metrics.TotalActionsProcessed += actionsProcessed;
            Metrics.AverageProcessingTimeMs = totalProcessingTime / actionsProcessed;
            Metrics.MaxProcessingTimeMs = Math.Max(Metrics.MaxProcessingTimeMs, maxProcessingTime);
        }
        else
        {
            Metrics.ActionsProcessedInTick = 0;
        }

        UpdatePriorityMetrics();
    }

    /// <summary>
    /// Updates the priority counts in the metrics.
    /// </summary>
    private void UpdatePriorityMetrics()
    {
        if (_config.EnableDetailedMetrics)
        {
            Metrics.HighPriorityCount = _priorityQueues[EventLoopPriority.High].Count;
            Metrics.NormalPriorityCount = _priorityQueues[EventLoopPriority.Normal].Count;
            Metrics.LowPriorityCount = _priorityQueues[EventLoopPriority.Low].Count;
            Metrics.QueuedActionsCount = GetTotalQueuedActionsCount();
        }
    }

    /// <summary>
    /// Gets the total number of queued actions.
    /// </summary>
    private int GetTotalQueuedActionsCount()
    {
        return _priorityQueues.Values.Sum(q => q.Count) + _delayedActions.Count;
    }

    /// <summary>
    /// Disposes resources used by the event loop service.
    /// </summary>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        // Make sure the event loop is stopped
        if (_isRunning)
        {
            StopAsync().GetAwaiter().GetResult();
        }

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        _isDisposed = true;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Class that tracks a queued action to facilitate cancellation.
    /// </summary>
    private class QueuedActionReference
    {
        public EventLoopPriority Priority { get; }
        public Guid ActionId { get; }

        public QueuedActionReference(EventLoopPriority priority, Guid actionId)
        {
            Priority = priority;
            ActionId = actionId;
        }
    }

    /// <summary>
    /// Class that represents a delayed action.
    /// </summary>
    private class DelayedAction
    {
        public QueuedAction Action { get; }
        public DateTime ExecuteAt { get; }

        public DelayedAction(QueuedAction action, DateTime executeAt)
        {
            Action = action;
            ExecuteAt = executeAt;
        }
    }
}

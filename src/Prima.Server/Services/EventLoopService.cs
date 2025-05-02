using System.Collections.Concurrent;
using System.Diagnostics;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Config.Internal.EventLoop;
using Prima.Core.Server.Data.Metrics.EventLoop;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Server.Services;

/// <summary>
/// Implementation of the event loop service that manages execution of actions with different priorities.
/// The event loop runs in a dedicated background task.
/// </summary>
public class EventLoopService : IEventLoopService, IDisposable
{
    private readonly ILogger<EventLoopService> _logger;
    private readonly IEventBusService _eventBusService;
    private readonly EventLoopConfig _config;
    private readonly ConcurrentDictionary<Guid, QueuedAction> _actionQueue = new();
    private readonly ConcurrentDictionary<Guid, (DateTime ExecuteAt, QueuedAction Action)> _delayedActions = new();
    private readonly object _tickLock = new();
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _loopTask;
    private long _lastTickTimestamp;
    private bool _isRunning;
    private bool _isDisposed;

    public event IEventLoopService.EventLoopTickHandler? OnTick;
    public event IEventLoopService.EventLoopResetHandler? OnTickReset;

    /// <summary>
    /// Gets or sets the interval in milliseconds between each tick of the event loop.
    /// </summary>
    public int TickIntervalMs
    {
        get => _config.TickIntervalMs;
        set => _config.TickIntervalMs = value;
    }

    /// <summary>
    /// Gets the current metrics of the event loop.
    /// </summary>
    public EventLoopMetrics Metrics { get; } = new EventLoopMetrics();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventLoopService"/> class.
    /// </summary>
    /// <param name="logger">The logger for this service.</param>
    /// <param name="eventBusService">The event bus service for publishing events.</param>
    /// <param name="config">The configuration for the event loop.</param>
    public EventLoopService(ILogger<EventLoopService> logger, IEventBusService eventBusService, EventLoopConfig config)
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _config = config;
    }

    /// <summary>
    /// Starts the event loop service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
            return Task.CompletedTask;

        _isRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _loopTask = Task.Run(EventLoopAsync, _cancellationTokenSource.Token);

        _logger.LogInformation("EventLoopService started with tick interval of {TickIntervalMs}ms", TickIntervalMs);
        return Task.CompletedTask;
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
                // If processing took longer than the interval, don't delay
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
    /// <param name="cancellationToken">The cancellation token.</param>
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
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <returns>The ID of the queued action.</returns>
    public Guid EnqueueAction(string name, Action action)
    {
        return EnqueueAction(name, action, EventLoopPriority.Normal);
    }

    /// <summary>
    /// Enqueues an action to be executed with the specified priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <returns>The ID of the queued action.</returns>
    public Guid EnqueueAction(string name, Action action, EventLoopPriority priority)
    {
        var queuedAction = new QueuedAction(name, action, priority);
        _actionQueue[queuedAction.Id] = queuedAction;

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
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="delay">The delay before executing the action.</param>
    /// <returns>The ID of the queued action.</returns>
    public Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay)
    {
        return EnqueueDelayedAction(name, action, delay, EventLoopPriority.Normal);
    }

    /// <summary>
    /// Enqueues an action to be executed after the specified delay with the specified priority.
    /// </summary>
    /// <param name="name">The name of the action for identification.</param>
    /// <param name="action">The action to execute.</param>
    /// <param name="delay">The delay before executing the action.</param>
    /// <param name="priority">The priority of the action.</param>
    /// <returns>The ID of the queued action.</returns>
    public Guid EnqueueDelayedAction(string name, Action action, TimeSpan delay, EventLoopPriority priority)
    {
        var queuedAction = new QueuedAction(name, action, priority);
        var executeAt = DateTime.UtcNow.Add(delay);
        _delayedActions[queuedAction.Id] = (executeAt, queuedAction);

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
    /// <param name="actionId">The ID of the action to cancel.</param>
    /// <returns>True if the action was found and canceled; otherwise, false.</returns>
    public bool TryCancelAction(Guid actionId)
    {
        bool removed = _actionQueue.TryRemove(actionId, out var action);

        if (!removed)
        {
            removed = _delayedActions.TryRemove(actionId, out var delayedAction);
            if (removed)
            {
                action = delayedAction.Action;
            }
        }

        if (removed)
        {
            UpdatePriorityMetrics();
            _logger.LogTrace("Action '{Name}' with ID {Id} was cancelled", action.Name, actionId);
        }

        return removed;
    }

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

                Metrics.AverageTimeBetweenTicksMs = (Metrics.AverageTimeBetweenTicksMs * 0.9) + (timeSinceLastTick * 0.1);
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
            Metrics.QueuedActionsCount = _actionQueue.Count + _delayedActions.Count;

            // Log action processing stats periodically (mantenuto dal tuo codice)
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
            if (_delayedActions.TryRemove(actionEntry.Key, out _))
            {
                _actionQueue[actionEntry.Value.Action.Id] = actionEntry.Value.Action;

                _logger.LogTrace(
                    "Delayed action '{Name}' with ID {Id} is now ready for execution",
                    actionEntry.Value.Action.Name,
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
        if (_actionQueue.IsEmpty)
            return;

        // Group actions by priority and sort them
        var actionsByPriority = _actionQueue.Values
            .GroupBy(a => a.Priority)
            .OrderByDescending(g => g.Key) // Process higher priorities first
            .ToList();

        var actionsProcessed = 0;
        var totalProcessingTime = 0.0;
        var maxProcessingTime = 0.0;
        var maxActionsToProcess = _config.MaxActionsPerTick;

        foreach (var priorityGroup in actionsByPriority)
        {
            foreach (var queuedAction in priorityGroup)
            {
                if (!_isRunning || actionsProcessed >= maxActionsToProcess)
                    return;

                if (_actionQueue.TryRemove(queuedAction.Id, out var action))
                {
                    try
                    {
                        _logger.LogTrace("Executing action '{Name}' with ID {Id}", action.Name, action.Id);

                        // Create mutable copy of the struct to update timestamps
                        var mutableAction = action;

                        // Record start timestamp
                        mutableAction.ExecutionStartTimestamp = Stopwatch.GetTimestamp();

                        // Execute the action
                        action.Action.Invoke();

                        // Record end timestamp
                        mutableAction.ExecutionEndTimestamp = Stopwatch.GetTimestamp();

                        // Calculate execution time
                        var processingTime = Stopwatch.GetElapsedTime(
                                mutableAction.ExecutionStartTimestamp,
                                mutableAction.ExecutionEndTimestamp
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
            Metrics.HighPriorityCount = _actionQueue.Values.Count(a => a.Priority == EventLoopPriority.High);
            Metrics.NormalPriorityCount = _actionQueue.Values.Count(a => a.Priority == EventLoopPriority.Normal);
            Metrics.LowPriorityCount = _actionQueue.Values.Count(a => a.Priority == EventLoopPriority.Low);
            Metrics.QueuedActionsCount = _actionQueue.Count + _delayedActions.Count;
        }
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
}

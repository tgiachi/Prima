using System.Collections.Concurrent;
using Orion.Foundations.Pool;
using Prima.Core.Server.Data.Internal;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class TimerService : ITimerService
{
    private readonly ILogger _logger;
    private readonly IEventLoopService _eventLoopService;

    private readonly ObjectPool<TimerDataObject> _timerDataPool = new(5);

    private readonly SemaphoreSlim _timerSemaphore = new(1, 1);
    private readonly BlockingCollection<TimerDataObject> _timers = new();

    public TimerService(ILogger<TimerService> logger, IEventLoopService eventLoopService)
    {
        _logger = logger;
        _eventLoopService = eventLoopService;
    }

    private void EventLoopServiceOnOnTick(double tickDurationMs)
    {
        _timerSemaphore.Wait();

        foreach (var timer in _timers)
        {
            timer.DecrementRemainingTime(tickDurationMs);

            if (timer.RemainingTimeInMs <= 0)
            {
                try
                {
                    timer.Callback?.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing timer callback for {TimerId}", timer.Id);
                }

                if (timer.Repeat)
                {
                    timer.ResetRemainingTime();
                }
                else
                {
                    _timers.TryTake(out var _);
                    _logger.LogInformation("Unregistering timer: {TimerId}", timer.Id);
                }
            }
        }

        _timerSemaphore.Release();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _eventLoopService.OnTick += EventLoopServiceOnOnTick;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public string RegisterTimer(string name, int intervalInSeconds, Action callback, bool repeat = false)
    {
        var existingTimer = _timers.FirstOrDefault(t => t.Name == name);

        if (existingTimer != null)
        {
            _logger.LogWarning("Timer with name {Name} already exists. Unregistering it.", name);
            UnregisterTimer(existingTimer.Id);
        }

        _timerSemaphore.Wait();

        var timerId = Guid.NewGuid().ToString();
        var timer = _timerDataPool.Get();

        timer.Name = name;
        timer.Id = timerId;
        timer.IntervalInMs = TimeSpan.FromMilliseconds(intervalInSeconds).TotalMilliseconds;
        timer.Callback = callback;
        timer.Repeat = repeat;
        timer.RemainingTimeInMs = TimeSpan.FromMilliseconds(intervalInSeconds).TotalMilliseconds;
        timer.RemainingTimeInMs = TimeSpan.FromMilliseconds(intervalInSeconds).TotalMilliseconds;


        _timers.Add(timer);

        _timerSemaphore.Release();

        _logger.LogDebug(
            "Registering timer: {TimerId}, Interval: {IntervalInSeconds} seconds, Repeat: {Repeat}",
            timerId,
            intervalInSeconds,
            repeat
        );

        return timerId.ToString();
    }

    public void UnregisterTimer(string timerId)
    {
        _timerSemaphore.Wait();

        var timer = _timers.FirstOrDefault(t => t.Id == timerId);

        if (timer != null)
        {
            _timers.TryTake(out timer);
            _logger.LogInformation("Unregistering timer: {TimerId}", timer.Id);
            _timerDataPool.Return(timer);
        }
        else
        {
            _logger.LogWarning("Timer with ID {TimerId} not found", timerId);
        }

        _timerSemaphore.Release();
    }

    public void UnregisterAllTimers()
    {
        _timerSemaphore.Wait();

        while (_timers.TryTake(out var timer))
        {
            _logger.LogInformation("Unregistering timer: {TimerId}", timer.Id);
        }

        _timerSemaphore.Release();
    }

    public void Dispose()
    {
        _timerSemaphore.Dispose();
        _timers.Dispose();

        _eventLoopService.OnTick -= EventLoopServiceOnOnTick;

        GC.SuppressFinalize(this);
    }
}

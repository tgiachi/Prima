using Orion.Core.Server.Attributes.Scripts;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Modules.Scripts;

[ScriptModule("timers")]
public class TimerScriptModule
{
    private readonly ITimerService _timerService;

    public TimerScriptModule(ITimerService timerService)
    {
        _timerService = timerService;
    }


    [ScriptFunction("Register a timer")]
    public string Register(
        string name, int intervalInSeconds, Action callback, int delayInSeconds = 0, bool isRepeat = false
    )
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "Timer name cannot be null or empty");
        }

        if (intervalInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(intervalInSeconds), "Interval must be greater than zero");
        }

        if (callback == null)
        {
            throw new ArgumentNullException(nameof(callback), "Callback cannot be null");
        }

        return _timerService.RegisterTimer(name, intervalInSeconds, callback, delayInSeconds, isRepeat);
    }

    [ScriptFunction("Register a timer that repeats")]
    public string Repeated(string name, int intervalInSeconds, Action callback, int delayInSeconds = 0)
    {
        return Register(name, intervalInSeconds, callback, delayInSeconds, true);
    }


    [ScriptFunction("Register a timer that runs once")]
    public string OneShot(string name, int intervalInSeconds, Action callback, int delayInSeconds = 0)
    {
        return Register(name, intervalInSeconds, callback, delayInSeconds, false);
    }

    [ScriptFunction("Unregister a timer")]
    public void Unregister(string timerId)
    {
        if (string.IsNullOrEmpty(timerId))
        {
            throw new ArgumentNullException(nameof(timerId), "Timer ID cannot be null or empty");
        }

        _timerService.UnregisterTimer(timerId);
    }
}

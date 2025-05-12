using Orion.Core.Server.Interfaces.Services.Base;

namespace Prima.Core.Server.Interfaces.Services;

public interface ITimerService : IOrionService, IOrionStartService, IDisposable
{
    string RegisterTimer(string name, double intervalInMs, Action callback, double delayInMs = 0, bool repeat = false);

    void UnregisterTimer(string timerId);

    void UnregisterAllTimers();
}

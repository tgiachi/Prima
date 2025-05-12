using Orion.Core.Server.Interfaces.Services.Base;

namespace Prima.Core.Server.Interfaces.Services;

public interface ITimerService : IOrionService, IOrionStartService, IDisposable
{
    string RegisterTimer(string name, int intervalInSeconds, Action callback, bool repeat = false);

    void UnregisterTimer(string timerId);

    void UnregisterAllTimers();
}

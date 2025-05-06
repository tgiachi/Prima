using Orion.Core.Server.Attributes.Scripts;
using Orion.Core.Server.Interfaces.Services.System;

namespace Prima.Server.Modules.Scripts;

[ScriptModule("events")]
public class EventScriptModule
{
    private readonly IScriptEngineService _scriptEngineService;

    private readonly IEventDispatcherService _eventDispatcherService;

    public EventScriptModule(IScriptEngineService scriptEngineService, IEventDispatcherService eventDispatcherService)
    {
        _scriptEngineService = scriptEngineService;
        _eventDispatcherService = eventDispatcherService;
    }


    [ScriptFunction("Register a callback to be called when the script prima server   is started")]
    public void OnStarted(Action action)
    {
        _scriptEngineService.AddCallback("onStarted", _ => action());
    }

    [ScriptFunction("Hook into an event")]
    public void HookEvent(string eventName, Action<object?> eventHandler)
    {
        _eventDispatcherService.SubscribeToEvent(eventName, eventHandler.Invoke);
    }
}

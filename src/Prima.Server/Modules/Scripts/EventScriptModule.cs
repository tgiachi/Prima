using Orion.Core.Server.Attributes.Scripts;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Contexts;
using Prima.UOData.Data.EventData;

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

    [ScriptFunction("Register a callback when the user logs in")]
    public void OnUserLogin(Action<UserLoginContext> action)
    {
        _scriptEngineService.AddCallback(
            nameof(OnUserLogin),
            context =>
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context), "Context cannot be null");
                    return;
                }

                if (context[0] is UserLoginContext userLoginContext)
                {
                    action(userLoginContext);
                }
            }
        );
    }

    [ScriptFunction("Register a callback when  user creates a character")]
    public void OnCharacterCreated(Action<CharacterCreatedEventArgs> action)
    {
        _scriptEngineService.AddCallback(
            nameof(OnCharacterCreated),
            context =>
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context), "Context cannot be null");
                    return;
                }

                if (context[0] is CharacterCreatedEventArgs characterCreatedEventArgs)
                {
                    action(characterCreatedEventArgs);
                }
            }
        );
    }
}

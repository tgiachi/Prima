using Prima.Core.Server.Attributes.Scripts;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Server.Modules.Scripts;

[ScriptModule("commands")]
public class CommandsScriptModule
{
    private readonly ICommandSystemService _commandSystemService;


    private readonly ILogger _logger;

    public CommandsScriptModule(ICommandSystemService commandSystemService, ILogger<CommandsScriptModule> logger)
    {
        _commandSystemService = commandSystemService;
        _logger = logger;
    }

    /// <summary>
    ///  Registers a command that can be executed in the console.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="description"></param>
    /// <param name="execute"></param>
    /// <param name="aliases"></param>
    /// <param name="type"></param>
    /// <param name="permission"></param>
    /// <returns></returns>
    ///
    [ScriptFunction("Register a console command")]
    public bool RegisterConsoleCommand(
        string command, string description, Action<string[]> execute, string[] aliases = null,
        CommandType type = CommandType.Console, CommandPermissionType permission = CommandPermissionType.Admin
    )
    {
        return _commandSystemService.RegisterCommand(
            command,
            description,
            null,
            strings =>
            {
                _logger.LogDebug("JS Executing command: {Command}", command);
                execute(strings);

                return Task.CompletedTask;
            },
            aliases,
            type,
            permission
        );
    }
}

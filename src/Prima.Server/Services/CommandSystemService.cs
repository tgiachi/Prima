using Prima.Core.Server.Data.Commands;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Server.Services;

public class CommandSystemService : ICommandSystemService
{
    private readonly ILogger _logger;

    private readonly Dictionary<string, CommandDefinitionData> _commandIndex = new();

    public CommandSystemService(ILogger<CommandSystemService> logger)
    {
        _logger = logger;
    }


    public string[] AutoComplete(string query)
    {
        return _commandIndex.Keys
            .Where(k => k.StartsWith(query, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .OrderBy(k => k)
            .ToArray();
    }

    public bool RegisterCommand(
        string command, string description, Dictionary<string, string> arguments, Func<string[], Task> execute,
        string[]? aliases = null,
        CommandType type = CommandType.InGame, CommandPermissionType permission = CommandPermissionType.Admin
    )
    {
        if (string.IsNullOrEmpty(command))
        {
            _logger.LogError("Command cannot be null or empty");
            return false;
        }


        var commandDefinition = new CommandDefinitionData
        {
            Command = command,
            Description = description,
            Arguments = arguments.Select(a => a.Key).ToArray(),
            Aliases = aliases ?? [],
            Type = type,
            Permission = permission
        };

        _commandIndex[commandDefinition.Command] = commandDefinition;


        foreach (var alias in commandDefinition.Aliases)
        {
            _commandIndex[alias] = commandDefinition;
        }

        return true;
    }

    public async Task<CommandResult> ExecuteCommandAsync(
        string command, CommandType commandSource, CommandPermissionType permission = CommandPermissionType.None
    )
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return CommandResult.Error("Command cannot be null or empty");
        }


        var commandParts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var commandName = commandParts[0];
        if (!_commandIndex.TryGetValue(commandName, out var commandDefinition))
        {
            return CommandResult.Error($"Command '{commandName}' not found");
        }

        if ((commandDefinition.Permission & permission) == 0)
        {
            return CommandResult.Error($"Permission denied for command '{commandName}'");
        }

        if (!commandDefinition.Type.HasFlag(commandSource))
        {
            return CommandResult.Error($"Command '{commandName}' cannot be executed from source: {commandSource}");
        }

        var commandArgs = commandParts.Skip(1).ToArray();


        try
        {
            await commandDefinition.Execute(commandArgs);
            return CommandResult.Success($"Command '{commandName}' executed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command '{CommandName}'", commandName);
            return CommandResult.Error($"Error executing command '{commandName}': {ex.Message}");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
    }

    public async Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
    }
}

using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Data.Commands;
using Prima.Core.Server.Types;

namespace Prima.Core.Server.Interfaces.Services;

public interface ICommandSystemService : IOrionService, IOrionStartService
{
    string[] AutoComplete(string query);

    bool RegisterCommand(
        string command, string description, Dictionary<string, string> arguments, Func<string[], Task> execute, string[]? aliases = null,
        CommandType type = CommandType.All, CommandPermissionType permission = CommandPermissionType.Admin
    );

    Task<CommandResult> ExecuteCommandAsync(
        string command, CommandType commandSource,  CommandPermissionType permission = CommandPermissionType.None
    );
}

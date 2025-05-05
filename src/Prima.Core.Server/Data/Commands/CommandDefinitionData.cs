using Prima.Core.Server.Types;

namespace Prima.Core.Server.Data.Commands;

public class CommandDefinitionData
{
    public string Command { get; set; }
    public string Description { get; set; }
    public string[] Arguments { get; set; }
    public string[] Aliases { get; set; }

    public CommandType Type { get; set; }
    public CommandPermissionType Permission { get; set; }

    public Func<string[], Task> Execute { get; set; } = null!;
}

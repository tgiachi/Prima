using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Config;
using Prima.Core.Server.Data.Config.Sections;
using JwtAuthConfig = Prima.Core.Server.Data.Config.Sections.JwtAuthConfig;

namespace Prima.Core.Server.Data.Config;

public class PrimaServerConfig : IOrionServerConfig
{
    public DebugConfig Debug { get; set; } = new();

    public ProcessConfig Process { get; set; } = new();

    public ShardConfig Shard { get; set; } = new();

    public JwtAuthConfig JwtAuth { get; set; } = new();

    public AccountServerConfig Accounts { get; set; } = new();

    public TcpServerConfig TcpServer { get; set; } = new();
}

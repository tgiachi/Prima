using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Config;
using Prima.Core.Server.Data.Config.Sections;

namespace Prima.Core.Server.Data.Config;

public class PrimaServerConfig : IOrionServerConfig
{
    public DebugConfig Debug { get; set; } = new();

    public ProcessConfig Process { get; set; } = new();

    public string UoDirectory { get; set; }

    public TcpServerConfig TcpServer { get; set; } = new();
}

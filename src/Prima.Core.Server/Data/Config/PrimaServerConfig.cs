using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Interfaces.Config;

namespace Prima.Core.Server.Data.Config;

public class PrimaServerConfig : IOrionServerConfig
{
    public DebugConfig Debug { get; set; } = new();

    public string UoDirectory { get; set; }
}

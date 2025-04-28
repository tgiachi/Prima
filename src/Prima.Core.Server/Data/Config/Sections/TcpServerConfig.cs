using Orion.Core.Server.Interfaces.Config;

namespace Prima.Core.Server.Data.Config.Sections;

public class TcpServerConfig : IOrionSectionConfig
{
    public string ServerName { get; set; } = "Prima Ultima Shard";
    public int LoginPort { get; set; } = 2593;

    public int GamePort { get; set; } = 2592;

    public void Load()
    {

    }

    public void BeforeSave()
    {

    }
}

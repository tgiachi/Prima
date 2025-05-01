using Orion.Core.Server.Interfaces.Config;

namespace Prima.Core.Server.Data.Config.Sections;

public class TcpServerConfig : IOrionSectionConfig
{
    public string Host { get; set; } = "";
    public int LoginPort { get; set; } = 2593;
    public int GamePort { get; set; } = 2592;

    public bool EnableWebServer { get; set; } = true;

    public int WebServerPort { get; set; } = 23000;


    public void Load()
    {
    }

    public void BeforeSave()
    {
    }
}

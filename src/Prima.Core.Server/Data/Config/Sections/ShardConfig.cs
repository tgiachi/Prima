namespace Prima.Core.Server.Data.Config.Sections;

public class ShardConfig
{
    public int MaxPlayers { get; set; } = 1000;

    public string UoDirectory { get; set; } = "";

    public string? ClientVersion { get; set; }

    public string Name { get; set; } = "Prima Shard";

    public string AdminEmail { get; set; } = "admin@primauo.com";

    public string Url { get; set; } = "https://github.com/tgiachi/prima";

    public int TimeZone { get; set; } = 0;

    public string Language { get; set; } = "en";
}

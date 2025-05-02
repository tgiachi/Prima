namespace Prima.Core.Server.Data.Config.Sections;

public class SmtpConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = false;
    public bool UseStartTls { get; set; } = false;
}

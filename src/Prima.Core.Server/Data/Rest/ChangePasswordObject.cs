namespace Prima.Core.Server.Data.Rest;

public class ChangePasswordObject
{
    public string AccountName { get; set; }

    public string OldPassword { get; set; }

    public string NewPassword { get; set; }
}

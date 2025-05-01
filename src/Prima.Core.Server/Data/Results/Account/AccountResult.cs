using LiteDB;

namespace Prima.Core.Server.Data.Results.Account;

public class AccountResult
{

    public bool IsSuccess { get; set; }

    public string? Message { get; set; }

    public string? Username { get; set; }


    public static AccountResult Success(string username)
    {
        return new AccountResult
        {
            IsSuccess = true,
            Username = username
        };
    }

    public static AccountResult Failure(string message)
    {
        return new AccountResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

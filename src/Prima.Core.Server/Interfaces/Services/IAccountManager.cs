using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Data.Rest;
using Prima.Core.Server.Data.Results.Account;
using Prima.Core.Server.Entities;

namespace Prima.Core.Server.Interfaces.Services;

public interface IAccountManager : IOrionService, IOrionStartService
{
    Task<AccountResult> CreateAccountAsync(string username, string password, string? email = null, bool admin = false, bool isVerified = false);
    Task<AccountEntity?> LoginGameAsync(string username, string password);
    Task<LoginResponseObject> LoginWebAsync(LoginRequestObject loginRequest);
    Task<AccountEntity> FindAccountByUsername(string username);
    Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword);
}

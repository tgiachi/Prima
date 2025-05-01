using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Data.Results.Account;
using Prima.Core.Server.Entities;

namespace Prima.Core.Server.Interfaces.Services;

public interface IAccountManager : IOrionService
{
    Task<AccountResult> CreateAccountAsync(string username, string password, string? email = null);
    Task<AccountEntity> LoginAsync(string username, string password);

    Task<AccountEntity> FindAccountByUsername(string username);
}

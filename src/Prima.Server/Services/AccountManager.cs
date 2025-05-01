using Prima.Core.Server.Data.Results.Account;
using Prima.Core.Server.Entities;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class AccountManager : IAccountManager
{
    public Task<AccountResult> CreateAccountAsync(string username, string password, string? email = null)
    {
        throw new NotImplementedException();
    }

    public Task<AccountEntity> LoginAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Task<AccountEntity> FindAccountByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

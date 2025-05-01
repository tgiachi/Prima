using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Utils;
using Prima.Core.Server.Data.Results.Account;
using Prima.Core.Server.Entities;
using Prima.Core.Server.Events.Account;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class AccountManager : IAccountManager
{
    private readonly IDatabaseService _databaseService;

    private readonly IEventBusService _eventBusService;
    private readonly ILogger _logger;

    public AccountManager(ILogger<AccountManager> logger, IDatabaseService databaseService, IEventBusService eventBusService)
    {
        _databaseService = databaseService;
        _eventBusService = eventBusService;
        _logger = logger;
    }

    public async Task<AccountResult> CreateAccountAsync(
        string username, string password, string? email = null, bool admin = false, bool isVerified = false
    )
    {
        var existingAccount = await FindAccountByUsername(username);

        if (existingAccount != null)
        {
            return AccountResult.Failure("Username already exists.");
        }


        var newAccount = new AccountEntity
        {
            Username = username,
            Email = email,
            HashedPassword = HashUtils.CreatePassword(password)
        };

        var result = await _databaseService.InsertAsync(newAccount);

        await _eventBusService.PublishAsync(new AccountCreatedEvent(result.Id.ToString(), result.Username, result.IsAdmin));


        return AccountResult.Success(result.Username);
    }

    public Task<AccountEntity> LoginAsync(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<AccountEntity?> FindAccountByUsername(string username)
    {
        var account = await _databaseService.FirstOrDefaultAsync<AccountEntity>(x => x.Username == username);

        return account;
    }

    public async Task<bool> ChangePasswordAsync(string username, string oldPassword, string newPassword)
    {
        var account = await FindAccountByUsername(username);

        if (account == null)
        {
            return false;
        }

        if (!HashUtils.VerifyPassword(oldPassword, account.HashedPassword))
        {
            return false;
        }

        account.HashedPassword = HashUtils.CreatePassword(newPassword);

        await _databaseService.UpdateAsync(account);

        return true;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        await CheckDefaultAdminUserAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private async Task CheckDefaultAdminUserAsync()
    {
        var usersCounts = await _databaseService.CountAsync<AccountEntity>();

        if (usersCounts > 0)
        {
            return;
        }

        var generatedPassword = HashUtils.GenerateRandomRefreshToken(8);

        var defaultAdminUser = new AccountEntity
        {
            Username = "admin",
            Email = "admin@admin.com",
            HashedPassword = generatedPassword,
            IsAdmin = true,
            IsVerified = true,
        };

        var result = await _databaseService.InsertAsync(defaultAdminUser);

        _logger.LogInformation("----------------------------");
        _logger.LogInformation(
            "Default admin user created with username: {Username} and password: {Password}",
            result.Username,
            generatedPassword
        );
        _logger.LogInformation("----------------------------");
    }
}

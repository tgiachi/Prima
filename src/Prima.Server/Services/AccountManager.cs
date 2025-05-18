using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Utils;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Rest;
using Prima.Core.Server.Data.Results.Account;
using Prima.Core.Server.Entities;
using Prima.Core.Server.Events.Account;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class AccountManager : IAccountManager
{
    private readonly IDatabaseService _databaseService;


    private readonly PrimaServerConfig _primaServerConfig;
    private readonly IEventBusService _eventBusService;
    private readonly ILogger _logger;

    public AccountManager(
        ILogger<AccountManager> logger, IDatabaseService databaseService, IEventBusService eventBusService,
        PrimaServerConfig primaServerConfig
    )
    {
        _databaseService = databaseService;
        _eventBusService = eventBusService;
        _primaServerConfig = primaServerConfig;
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

    public async Task<AccountEntity> LoginGameAsync(string username, string password)
    {
        var account = await FindAccountByUsername(username);

        if (account == null)
        {
            return null;
        }

        if (!HashUtils.VerifyPassword(password, account.HashedPassword))
        {
            return null;
        }

        return account;
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

        var generatedPassword = "admin";

        var defaultAdminUser = new AccountEntity
        {
            Username = "admin",
            Email = "admin@admin.com",
            HashedPassword = HashUtils.CreatePassword(generatedPassword),
            IsAdmin = true,
            IsVerified = true,
            IsActive = true
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


    private string GenerateJwtToken(AccountEntity user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_primaServerConfig.JwtAuth.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: _primaServerConfig.JwtAuth.Issuer,
            audience: _primaServerConfig.JwtAuth.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_primaServerConfig.JwtAuth.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<LoginResponseObject> LoginWebAsync(LoginRequestObject loginRequest)
    {
        AccountEntity userEntity = null;

        if (loginRequest.EmailOrUsername == null)
        {
            _logger.LogWarning("Login failed: Username and email are both null.");

            return new LoginResponseObject(null, null, null, "Login failed: Username and email are both null.", false);
        }


        userEntity = await _databaseService.FirstOrDefaultAsync<AccountEntity>(s => s.Email == loginRequest.EmailOrUsername);

        if (userEntity == null)
        {
            userEntity = await _databaseService.FirstOrDefaultAsync<AccountEntity>(s =>
                s.Username == loginRequest.EmailOrUsername
            );
        }

        if (userEntity == null)
        {
            _logger.LogWarning("Login failed: User not found.");

            return new LoginResponseObject(null, null, null, "Login failed: User not found.", false);
        }

        if (!userEntity.IsAdmin)
        {
            _logger.LogWarning("Login failed: User is not an admin.");

            return new LoginResponseObject(null, null, null, "Login failed: User is not an admin.", false);
        }

        var cleanedPassword = userEntity.HashedPassword.Replace("hash:", "");
        var passwordHash = cleanedPassword.Split(":")[0];
        var salt = cleanedPassword.Split(":")[1];

        var isOk = HashUtils.VerifyPassword(loginRequest.Password, passwordHash + ":" + salt);

        if (!isOk)
        {
            _logger.LogWarning("Login failed: Invalid password.");

            return new LoginResponseObject(null, null, null, "Login failed: Invalid password.", false);
        }

        var token = GenerateJwtToken(userEntity);
        var refreshToken = HashUtils.GenerateRandomRefreshToken(64);

        userEntity.RefreshToken = refreshToken;
        userEntity.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_primaServerConfig.JwtAuth.RefreshTokenExpiryDays);

        await _databaseService.UpdateAsync(userEntity);


        _logger.LogInformation("Login successful for user: {Email}", userEntity.Email);


        return new LoginResponseObject(
            token,
            refreshToken,
            userEntity.RefreshTokenExpiry,
            null,
            true
        );
    }
}

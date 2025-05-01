using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Orion.Foundations.Utils;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Rest;
using Prima.Core.Server.Entities;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class AuthService : IAuthService
{
    private readonly ILogger _logger;
    private readonly IDatabaseService _databaseService;

    private readonly PrimaServerConfig _primaServerConfig;

    public AuthService(ILogger<AuthService> logger, IDatabaseService databaseService, PrimaServerConfig primaServerConfig)
    {
        _databaseService = databaseService;
        _primaServerConfig = primaServerConfig;
        _logger = logger;
    }

    public async Task<List<AccountEntity>> GetAllUsersAsync()
    {
        var result = await _databaseService.FindAllAsync<AccountEntity>();

        return result.ToList();
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

    public async Task<LoginResponseObject> LoginAsync(LoginRequestObject loginRequest)
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

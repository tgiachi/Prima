using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Data.Rest;
using Prima.Core.Server.Entities;

namespace Prima.Core.Server.Interfaces.Services;

public interface IAuthService : IOrionService
{
    Task<List<AccountEntity>> GetAllUsersAsync();

    Task<LoginResponseObject> LoginAsync(LoginRequestObject loginRequest);
}

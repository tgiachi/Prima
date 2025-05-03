using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Prima.Core.Server.Interfaces.Services;
using Prima.Server.Services;

namespace Prima.Server.Modules.Container;

public class AuthServicesModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
                .AddService<IAccountManager, AccountManager>()
            ;
    }
}

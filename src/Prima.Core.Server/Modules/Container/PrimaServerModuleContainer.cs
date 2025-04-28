using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Services;

namespace Prima.Core.Server.Modules.Container;

public class PrimaServerModuleContainer : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services.AddService<INetworkService, NetworkService>();
    }
}

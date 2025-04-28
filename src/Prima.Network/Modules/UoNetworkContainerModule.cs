using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Prima.Network.Interfaces.Services;
using Prima.Network.Services;

namespace Prima.Network.Modules;

public class UoNetworkContainerModule : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return services
                .AddService<IPacketManager, PacketManager>()
            ;
    }
}

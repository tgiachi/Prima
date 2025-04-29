using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Interfaces.Modules;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Interfaces.Sessions;
using Orion.Core.Server.Services;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Services;

namespace Prima.Core.Server.Modules.Container;

public class PrimaServerModuleContainer : IOrionContainerModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        return
            services
                .AddService<INetworkService, NetworkService>()
                .AddService<INetworkSessionService<NetworkSession>, NetworkSessionService<NetworkSession>>()
            ;
    }
}

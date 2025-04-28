using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Hosted;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Internal.Services;

namespace Prima.Server.Hosted;

public class PrimaHostedService : BaseOrionHostedService
{
    public PrimaHostedService(
        ILogger<BaseOrionHostedService> logger, List<ServiceDefinitionObject> serviceDefinitions,
        IEventBusService eventBusService, IServiceProvider serviceProvider
    ) : base(logger, serviceDefinitions, eventBusService, serviceProvider)
    {
    }


    protected override async Task BeforeStartAsync()
    {
        var directoriesConfig = ServiceProvider.GetRequiredService<DirectoriesConfig>();
        Logger.LogInformation("Root directory: {DirectoriesConfig}", directoriesConfig.Root);
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Modules.Container;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Services;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Options;
using Prima.Core.Server.Modules.Container;
using Prima.Core.Server.Types;
using Prima.Network.Modules;
using Prima.Server.Hosted;
using Prima.Server.Modules.Container;
using Serilog;

namespace Prima.Server;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var appContext = builder.Services.InitApplication<PrimaServerOptions, PrimaServerConfig>("prima", Enum.GetNames<DirectoryType>());

        builder.Services.AddSingleton(appContext);

        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        appContext.Options.ShowHeader(typeof(Program).Assembly);

        appContext.Config.SaveConfig(appContext.ConfigFilePath);


        builder.Services
            .AddModule<DefaultOrionServiceModule>()
            .AddModule<DefaultOrionScriptsModule>()
            .AddModule<UoNetworkContainerModule>()
            .AddModule<PrimaServerModuleContainer>()
            .AddModule<DatabaseModule>()
            .AddService<INetworkTransportManager, NetworkTransportManager>()
            .AddSingleton(
                new EventBusConfig()
                {
                    MaxConcurrentTasks = 4
                }
            )
            ;

        builder.Services.AddHostedService<PrimaHostedService>();


        var app = builder.Build();


        await app.RunAsync();
    }
}

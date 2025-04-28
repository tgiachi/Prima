using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Modules.Container;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Options;
using Serilog;

namespace Prima.Server;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var appContext = builder.Services.InitApplication<PrimaServerOptions, PrimaServerConfig>("prima");

        builder.Services.AddSingleton(appContext);

        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        appContext.Options.ShowHeader(typeof(Program).Assembly);


        appContext.Config.SaveConfig(appContext.ConfigFilePath);


        builder.Services
            .AddModule<DefaultOrionServiceModule>()
            .AddModule<DefaultOrionScriptsModule>();


        var app = builder.Build();


        await app.RunAsync();
    }
}

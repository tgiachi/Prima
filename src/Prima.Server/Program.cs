using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Orion.Core.Server.Data.Config.Internal;
using Orion.Core.Server.Data.Config.Sections;
using Orion.Core.Server.Extensions;
using Orion.Core.Server.Modules.Container;
using Orion.Foundations.Utils;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Core.Services;
using Prima.Core.Server.Data;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Config.Internal.EventLoop;
using Prima.Core.Server.Data.Options;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Modules.Container;
using Prima.Core.Server.Types;
using Prima.Network.Modules;
using Prima.Server.Handlers;
using Prima.Server.Hosted;
using Prima.Server.Modules.Container;
using Prima.Server.Routes;
using Prima.Server.Services;
using Serilog;

namespace Prima.Server;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var appContext =
            builder.Services.InitApplication<PrimaServerOptions, PrimaServerConfig>("prima", Enum.GetNames<DirectoryType>());

        builder.Services.AddSingleton(appContext);

        Log.Logger = appContext.LoggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders().AddSerilog();

        appContext.Options.ShowHeader(typeof(Program).Assembly);

        appContext.Config.SaveConfig(appContext.ConfigFilePath);

        InitJwtAuth(builder.Services, appContext.Config);


        builder.Services
            .AddEventBusService()
            .AddProcessQueueService()
            .AddScriptEngineService()
            .AddDiagnosticService(
                new DiagnosticServiceConfig()
                {
                    MetricsIntervalInSeconds = 60,
                    PidFileName = "prima_server.pid",
                }
            );

        builder.Services
            .AddModule<DefaultOrionServiceModule>()
            .AddModule<DefaultOrionScriptsModule>()
            .AddModule<UoNetworkContainerModule>()
            .AddModule<PrimaServerModuleContainer>()
            .AddModule<AuthServicesModule>()
            .AddModule<DatabaseModule>()
            .AddService<IEventLoopService, EventLoopService>()
            .AddSingleton(new EventLoopConfig())
            .AddService<INetworkTransportManager, NetworkTransportManager>();

        builder.Services
            .AddService<ConnectionHandler>()
            .AddService<LoginHandler>();

        builder.Services.AddHostedService<PrimaHostedService>();

        builder.Services.AddAuthorization();

        builder.Services.ConfigureHttpJsonOptions(o =>
            {
                o.SerializerOptions.DefaultIgnoreCondition = JsonUtils.GetDefaultJsonSettings().DefaultIgnoreCondition;
                o.SerializerOptions.Converters.Add(JsonUtils.GetDefaultJsonSettings().Converters[0]);
                o.SerializerOptions.WriteIndented = JsonUtils.GetDefaultJsonSettings().WriteIndented;
                o.SerializerOptions.PropertyNamingPolicy =
                    JsonUtils.GetDefaultJsonSettings().PropertyNamingPolicy;

                o.SerializerOptions.PropertyNameCaseInsensitive =
                    JsonUtils.GetDefaultJsonSettings().PropertyNameCaseInsensitive;
            }
        );

        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();


        Environment.SetEnvironmentVariable("PRIMA_HTTP_PORT", appContext.Config.TcpServer.WebServerPort.ToString());

        builder.WebHost.ConfigureKestrel(s =>
            {
                s.AddServerHeader = false;
                var ipAddress = appContext.Config.TcpServer.EnableWebServer ? IPAddress.Any : IPAddress.Loopback;
                s.Listen(ipAddress, appContext.Config.TcpServer.WebServerPort);

                Log.Logger.Information("Listening on {ipAddress}", ipAddress);
            }
        );

        var app = builder.Build();

        PrimaServerContext.ServiceProvider = app.Services;

        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Prima UO server API v1"); }
        );

        var group = app.MapGroup("api/v1")
            .WithName("Prima API V1")
            .WithTags("Prima API V1");

        group
            .MapAuthRoutes()
            .MapAccountRoutes()
            .MapMetricsRoutes()
            .MapStatusRoutes()
            ;

        await app.RunAsync();
    }

    private static void InitJwtAuth(IServiceCollection services, PrimaServerConfig config)
    {
        IdentityModelEventSource.ShowPII = true;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config.JwtAuth.Issuer,
                        ValidAudience = config.JwtAuth.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JwtAuth.Secret)),
                    };
                }
            );

        services.AddAuthorization();
    }
}

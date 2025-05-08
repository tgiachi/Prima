using Microsoft.Extensions.DependencyInjection;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Core.Server.Data;

public static class PrimaServerContext
{
    public static ClientVersion ClientVersion { get; set; }

    public static IServiceProvider ServiceProvider { get; set; }

    public static IEventLoopService EventLoopService => ServiceProvider.GetRequiredService<IEventLoopService>();

    public static INetworkSessionService<NetworkSession> NetworkSessionService => ServiceProvider.GetRequiredService<INetworkSessionService<NetworkSession>>();
}

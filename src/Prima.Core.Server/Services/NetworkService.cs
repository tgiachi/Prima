using System.Net;
using Microsoft.Extensions.Logging;
using Orion.Foundations.Types;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Interfaces.Services;

namespace Prima.Core.Server.Services;

public class NetworkService : INetworkService
{
    private readonly ILogger _logger;
    private readonly PrimaServerConfig _serverConfig;

    private readonly INetworkTransportManager _networkTransportManager;

    public NetworkService(
        ILogger<NetworkService> logger, PrimaServerConfig serverConfig, INetworkTransportManager networkTransportManager
    )
    {
        _logger = logger;
        _serverConfig = serverConfig;
        _networkTransportManager = networkTransportManager;

    }

    public async Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Adding login server
        _networkTransportManager.AddTransport(
            new NonSecureTcpServer(ServerNetworkType.Servers, IPAddress.Any, _serverConfig.TcpServer.LoginPort)
        );

        _networkTransportManager.AddTransport(
            new NonSecureTcpServer(ServerNetworkType.Clients, IPAddress.Any, _serverConfig.TcpServer.GamePort)
        );

        _logger.LogInformation("Login server started on port {Port}", _serverConfig.TcpServer.LoginPort);
        _logger.LogInformation("Game server started on port {Port}", _serverConfig.TcpServer.GamePort);

        await _networkTransportManager.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Stopping network transport manager");
        await _networkTransportManager.StopAsync(cancellationToken);
        _logger.LogInformation("Network transport manager stopped");
    }
}

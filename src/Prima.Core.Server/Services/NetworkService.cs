using System.Net;
using Microsoft.Extensions.Logging;
using Orion.Foundations.Types;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Core.Server.Services;

public class NetworkService : INetworkService
{
    private readonly ILogger _logger;
    private readonly PrimaServerConfig _serverConfig;

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IPacketManager _packetManager;

    public NetworkService(
        ILogger<NetworkService> logger, PrimaServerConfig serverConfig, INetworkTransportManager networkTransportManager,
        IPacketManager packetManager
    )
    {
        _logger = logger;

        _serverConfig = serverConfig;
        _networkTransportManager = networkTransportManager;
        _packetManager = packetManager;

        RegisterPackets();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Adding login server
        _networkTransportManager.AddTransport(
            new NonSecureTcpServer("login",ServerNetworkType.Servers, IPAddress.Any, _serverConfig.TcpServer.LoginPort)
        );


        // Adding game server
        _networkTransportManager.AddTransport(
            new NonSecureTcpServer("game_server", ServerNetworkType.Clients, IPAddress.Any, _serverConfig.TcpServer.GamePort)
        );

        _logger.LogInformation("Login server started on port {Port}", _serverConfig.TcpServer.LoginPort);
        _logger.LogInformation("Game server started on port {Port}", _serverConfig.TcpServer.GamePort);

        await _networkTransportManager.StartAsync(cancellationToken);

    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping network transport manager");
        await _networkTransportManager.StopAsync(cancellationToken);
        _logger.LogInformation("Network transport manager stopped");
    }

    private void RegisterPackets()
    {
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<ConnectToGameServer>();
        _packetManager.RegisterPacket<SelectServer>();
        _packetManager.RegisterPacket<GameServerList>();
        _packetManager.RegisterPacket<LoginDenied>();
    }
}

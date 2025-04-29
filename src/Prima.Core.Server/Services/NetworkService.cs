using System.Net;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Extensions;
using Orion.Foundations.Observable;
using Orion.Foundations.Types;
using Orion.Network.Core.Data;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Core.Server.Services;

public class NetworkService : INetworkService
{
    private const string _listenersContext = "network_server_listeners";

    private const string _loginContext = "login_server";

    private const string _gameContext = "game_server";


    private readonly ILogger _logger;
    private readonly PrimaServerConfig _serverConfig;

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IPacketManager _packetManager;

    private readonly IProcessQueueService _processQueueService;


    private readonly INetworkSessionService<NetworkSession> _networkSessionService;
    private readonly CancellationTokenSource _messageCancellationTokenSource = new();
    private readonly IDisposable _channelObservableSubscription;
    private readonly Dictionary<byte, List<INetworkPacketListener>> _listeners = new();


    public NetworkService(
        ILogger<NetworkService> logger, PrimaServerConfig serverConfig, INetworkTransportManager networkTransportManager,
        IPacketManager packetManager, IProcessQueueService processQueueService,
        INetworkSessionService<NetworkSession> networkSessionService
    )
    {
        _logger = logger;

        _serverConfig = serverConfig;
        _networkTransportManager = networkTransportManager;
        _packetManager = packetManager;
        _processQueueService = processQueueService;
        _networkSessionService = networkSessionService;
        _processQueueService.EnsureContext(_listenersContext);

        RegisterPackets();
        var chanObservable = new ChannelObservable<NetworkMessageData>(networkTransportManager.IncomingMessages);

        _channelObservableSubscription = chanObservable.Subscribe(data => HandleIncomingMessages(data));


        _networkTransportManager.ClientConnected += NetworkTransportManagerOnClientConnected;
        _networkTransportManager.ClientDisconnected += NetworkTransportManagerOnClientDisconnected;
    }

    private void NetworkTransportManagerOnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        if (transportId == _loginContext)
        {
            _logger.LogInformation("Client disconnected from login server: {SessionId} => {Endpoint}", sessionId, endpoint);
            return;
        }

        if (transportId == _gameContext)
        {
            _logger.LogInformation("Client disconnected from game server: {SessionId} => {Endpoint}", sessionId, endpoint);
            return;
        }
    }

    private void NetworkTransportManagerOnClientConnected(string transportId, string sessionId, string endpoint)
    {
        if (transportId == _loginContext)
        {
            _logger.LogInformation("Client connected to login server: {SessionId} => {Endpoint}", sessionId, endpoint);

            var session = _networkSessionService.AddSession(sessionId);
        }
        else if (transportId == _gameContext)
        {
            _logger.LogInformation("Client connected to game server: {SessionId} => {Endpoint}", sessionId, endpoint);
        }
    }

    private async Task HandleIncomingMessages(NetworkMessageData data)
    {
        try
        {
            var packet = _packetManager.ReadPacket(data.Message);

            if (packet == null)
            {
                _logger.LogWarning(
                    "Received unknown packet with length {Length} => {Buffer}",
                    data.Message.Length,
                    data.Message.HumanizedContent()
                );

                return;
            }

            _logger.LogDebug("Received packet: {Packet}", packet);

            if (_listeners.TryGetValue(packet.OpCode, out var packetListeners))
            {
                foreach (var listener in packetListeners)
                {
                    try
                    {
                        _processQueueService.Enqueue(
                            _listenersContext,
                            async () => { await listener.OnMessageReceived(data.SessionId, packet); }
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while handling packet {PacketType}", packet.GetType().Name);
                    }
                }
            }
            else
            {
                _logger.LogWarning("No listeners registered for packet {PacketType}", packet.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling incoming message");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        // Adding login server
        _networkTransportManager.AddTransport(
            new NonSecureTcpServer(
                _loginContext,
                ServerNetworkType.Servers,
                IPAddress.Any,
                _serverConfig.TcpServer.LoginPort
            )
        );


        // Adding game server
        _networkTransportManager.AddTransport(
            new NonSecureTcpServer(_gameContext, ServerNetworkType.Clients, IPAddress.Any, _serverConfig.TcpServer.GamePort)
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

    public void RegisterPacketListener<TPacket>(INetworkPacketListener listener) where TPacket : IUoNetworkPacket, new()
    {
        var packet = new TPacket();
        _logger.LogInformation("Registering packet listener for {PacketType}", "0x" + packet.OpCode.ToString("X2"));

        if (!_listeners.TryGetValue(packet.OpCode, out var packetListeners))
        {
            packetListeners = new List<INetworkPacketListener>();
            _listeners.Add(packet.OpCode, packetListeners);
        }

        packetListeners.Add(listener);
    }

    private void RegisterPackets()
    {
        _packetManager.RegisterPacket<ClientVersion>();
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<ConnectToGameServer>();
        _packetManager.RegisterPacket<SelectServer>();
        _packetManager.RegisterPacket<GameServerList>();
        _packetManager.RegisterPacket<LoginDenied>();
    }

    public void Dispose()
    {
        _messageCancellationTokenSource.Dispose();
        _channelObservableSubscription?.Dispose();

        GC.SuppressFinalize(this);
    }
}

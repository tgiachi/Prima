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
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Core.Server.Services;

public class NetworkService : INetworkService
{
    private readonly ILogger _logger;
    private readonly PrimaServerConfig _serverConfig;

    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IPacketManager _packetManager;

    private readonly IProcessQueueService _processQueueService;


    private readonly CancellationTokenSource _messageCancellationTokenSource = new();
    private readonly ChannelObservable<NetworkMessageData> _channelObservable;
    private readonly Dictionary<byte, List<INetworkPacketListener>> _listeners = new();

    private readonly string _listenersContext = "network_server_listeners";

    public NetworkService(
        ILogger<NetworkService> logger, PrimaServerConfig serverConfig, INetworkTransportManager networkTransportManager,
        IPacketManager packetManager, IProcessQueueService processQueueService
    )
    {
        _logger = logger;

        _serverConfig = serverConfig;
        _networkTransportManager = networkTransportManager;
        _packetManager = packetManager;
        _processQueueService = processQueueService;
        _processQueueService.EnsureContext(_listenersContext);

        RegisterPackets();
        _channelObservable = new ChannelObservable<NetworkMessageData>(networkTransportManager.IncomingMessages);

        _channelObservable.Subscribe(data => HandleIncomingMessages(data));
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
            new NonSecureTcpServer("login", ServerNetworkType.Servers, IPAddress.Any, _serverConfig.TcpServer.LoginPort)
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
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<ConnectToGameServer>();
        _packetManager.RegisterPacket<SelectServer>();
        _packetManager.RegisterPacket<GameServerList>();
        _packetManager.RegisterPacket<LoginDenied>();
    }

    public void Dispose()
    {
        _messageCancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}

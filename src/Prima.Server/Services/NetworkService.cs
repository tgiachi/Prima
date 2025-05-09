using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Foundations.Extensions;
using Orion.Foundations.Observable;
using Orion.Foundations.Types;
using Orion.Network.Core.Data;
using Orion.Network.Core.Extensions;
using Orion.Network.Core.Interfaces.Services;
using Orion.Network.Tcp.Servers;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Extensions;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;
using Prima.Network.Compression;
using Prima.Network.Interfaces.Packets;
using Prima.Network.Interfaces.Services;
using Prima.Network.Packets;
using Prima.UOData.Packets;

namespace Prima.Server.Services;

public class NetworkService : INetworkService
{
    public bool LogPackets { get; set; }

    private const string _listenersContext = "network_server_listeners";


    private const string _loginContext = "login_server";

    private const string _gameContext = "game_server";

    private readonly ILogger _logger;
    private readonly PrimaServerConfig _serverConfig;


    private readonly DirectoriesConfig _directoriesConfig;
    private readonly INetworkTransportManager _networkTransportManager;
    private readonly IPacketManager _packetManager;
    private readonly IEventLoopService _eventLoopService;

    private readonly IProcessQueueService _processQueueService;


    private readonly INetworkSessionService<NetworkSession> _networkSessionService;
    private readonly CancellationTokenSource _messageCancellationTokenSource = new();
    private readonly IDisposable _channelObservableSubscription;
    private readonly Dictionary<byte, List<INetworkPacketListener>> _listeners = new();

    private readonly BlockingCollection<NetworkSession> _inSeedSessions = new();


    public NetworkService(
        ILogger<NetworkService> logger, PrimaServerConfig serverConfig, INetworkTransportManager networkTransportManager,
        IPacketManager packetManager, IProcessQueueService processQueueService,
        INetworkSessionService<NetworkSession> networkSessionService, IEventLoopService eventLoopService,
        DirectoriesConfig directoriesConfig
    )
    {
        _logger = logger;

        _serverConfig = serverConfig;

        LogPackets = _serverConfig.TcpServer.LogPackets;

        _networkTransportManager = networkTransportManager;
        _packetManager = packetManager;
        _processQueueService = processQueueService;
        _networkSessionService = networkSessionService;
        _eventLoopService = eventLoopService;
        _directoriesConfig = directoriesConfig;
        _processQueueService.EnsureContext(_listenersContext);

        RegisterPackets();

        var chanObservable = new ChannelObservable<NetworkMessageData>(networkTransportManager.IncomingMessages);

        _channelObservableSubscription = chanObservable.Subscribe(data => HandleIncomingMessages(data));


        _networkTransportManager.ClientConnected += NetworkTransportManagerOnClientConnected;
        _networkTransportManager.ClientDisconnected += NetworkTransportManagerOnClientDisconnected;

        _networkTransportManager.RawPacketIn += (id, transportId, data) =>
        {
            var transport = _networkTransportManager.GetTransport(transportId);


            _logger.LogDebug(
                "<- {Session} ({Size} bytes) {Transport} {Data}",
                id.ToShortSessionId(),
                data.Length,
                transport.Id,
                data.HumanizedContent(20)
            );
        };

        _networkTransportManager.RawPacketOut += (id, transportId, data) =>
        {
            var transport = _networkTransportManager.GetTransport(transportId);

            _logger.LogDebug(
                "-> {Session} ({Size} bytes) {Transport} {Data}",
                id.ToShortSessionId(),
                data.Length,
                transport.Id,
                data.HumanizedContent(20)
            );
        };
    }

    private void NetworkTransportManagerOnClientDisconnected(string transportId, string sessionId, string endpoint)
    {
        var session = _networkSessionService.GetSession(sessionId);

        session.OnSendPacket -= SendPacketViaEventLoop;
        session.OnDisconnect -= DisconnectSession;


        if (transportId == _loginContext)
        {
            _logger.LogInformation("Client disconnected from login server: {SessionId} => {Endpoint}", sessionId, endpoint);

            if (session.AuthId > 0)
            {
                _logger.LogTrace("Moving session {SessionId} to game server", sessionId);
                _inSeedSessions.Add(session);
            }

            return;
        }

        if (transportId == _gameContext)
        {
            _logger.LogInformation("Client disconnected from game server: {SessionId} => {Endpoint}", sessionId, endpoint);
            return;
        }

        _networkSessionService.RemoveSession(sessionId);
    }

    private void NetworkTransportManagerOnClientConnected(string transportId, string sessionId, string endpoint)
    {
        if (transportId == _loginContext)
        {
            _logger.LogInformation("Client connected to login server: {SessionId} => {Endpoint}", sessionId, endpoint);
        }
        else if (transportId == _gameContext)
        {
            _logger.LogInformation("Client connected to game server: {SessionId} => {Endpoint}", sessionId, endpoint);
        }

        var session = _networkSessionService.AddSession(sessionId);

        session.OnSendPacket += SendPacketViaEventLoop;
        session.OnDisconnect += DisconnectSession;
    }

    private async Task DisconnectSession(string id)
    {
        _eventLoopService.EnqueueAction(
            "disconnect_session_" + id,
            async () => { await _networkTransportManager.DisconnectAsync(id); }
        );
    }


    public Task SendPacket<TPacket>(string sessionId, TPacket packet) where TPacket : IUoNetworkPacket
    {
        return SendPacketInternal(sessionId, (IUoNetworkPacket)packet);
    }

    public void MoveLoginSessionToGameSession(string sessionId, int authId)
    {
        var inSeedSession = _inSeedSessions.FirstOrDefault(s => s.AuthId == (uint)authId);

        if (inSeedSession == null)
        {
            throw new Exception($"Session {sessionId} does not exist");
        }

        var newSession = _networkSessionService.GetSession(sessionId);

        newSession.AuthId = inSeedSession.AuthId;
        newSession.AccountId = inSeedSession.AccountId;
        newSession.ClientVersion = inSeedSession.ClientVersion;
        newSession.Seed = inSeedSession.Seed;
        newSession.FirstPacketReceived = inSeedSession.FirstPacketReceived;
        newSession.IsSeed = inSeedSession.IsSeed;

        _inSeedSessions.TryTake(out inSeedSession);
    }

    public Task SendPacketViaEventLoop<TPacket>(string sessionId, TPacket packet) where TPacket : IUoNetworkPacket
    {
        return SendPacketViaEventLoop(sessionId, (IUoNetworkPacket)packet);
    }


    private async Task SendPacketViaEventLoop(string sessionId, IUoNetworkPacket packet)
    {
        _eventLoopService.EnqueueAction(
            $"send_packet_{sessionId.ToShortSessionId()}_{packet.OpCode}",
            () => { SendPacketInternal(sessionId, packet); },
            EventLoopPriority.High
        );
    }

    private async Task SendPacketInternal(string sessionId, IUoNetworkPacket packet)
    {
        try
        {
            var session = _networkSessionService.GetSession(sessionId);

            var packetContent = _packetManager.WritePacket(packet);

            Span<byte> data = stackalloc byte[packetContent.Length];

            LogPacket(packet, sessionId.ToShortSessionId(), data, false, session.UseNetworkCompression);

            if (session.UseNetworkCompression)
            {
                NetworkCompression.Compress(packetContent, data);
            }
            else
            {
                data = packetContent;
            }


            await _networkTransportManager.EnqueueMessageAsync(
                new NetworkMessageData(sessionId, data.ToArray(), ServerNetworkType.None)
            );
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error while sending packet {PacketType} to session {SessionId}",
                packet.GetType().Name,
                sessionId
            );
        }
    }

    private Span<byte> CheckIfSessionKeyPacket(byte[] data)
    {
        if (data.Length == 69)
        {
            // Drop the first 4 bytes

            var packet = new byte[data.Length - 4];

            Buffer.BlockCopy(data, 4, packet, 0, data.Length - 4);

            return packet.AsSpan();
        }

        return data.AsSpan();
    }


    private async Task HandleIncomingMessages(NetworkMessageData data)
    {
        try
        {
            var buffer = CheckIfSessionKeyPacket(data.Message);


            var packets = _packetManager.ReadPackets(buffer.ToArray());

            if (packets.Count == 0)
            {
                _logger.LogWarning(
                    "Received unknown packet with length {Length} => {Buffer}",
                    data.Message.Length,
                    data.Message.HumanizedContent()
                );

                return;
            }


            foreach (var packet in packets)
            {
                _logger.LogDebug("Received packet: {Packet}", packet);
                LogPacket(packet, data.SessionId.ToShortSessionId(), data.Message, true, false);


                if (_listeners.TryGetValue(packet.OpCode, out var packetListeners))
                {
                    foreach (var listener in packetListeners)
                    {
                        try
                        {
                            _processQueueService.Enqueue(
                                _listenersContext,
                                async () => { await listener.OnPacketReceived(data.SessionId, packet); }
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

    public void LogPacket(
        IUoNetworkPacket? packet, string sessionId, ReadOnlySpan<byte> buffer, bool isIncoming, bool isCompressed
    )
    {
        if (!LogPackets)
        {
            return;
        }

        var packetsDirectoryPath = Path.Combine(_directoriesConfig[DirectoryType.Logs], "Packets", "packets.log");

        if (!File.Exists(packetsDirectoryPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(packetsDirectoryPath));
        }

        var packetType = "";

        if (packet != null)
        {
            packetType = $"0x{packet.OpCode:X2} - {packet.GetType().Name}";
        }
        else
        {
            byte opCode = 0x00;
            if (buffer.Length == 69)
            {
                // Drop the first 4 bytes
                opCode = buffer[4];
            }
            else if (buffer.Length > 0)
            {
                opCode = buffer[0];
            }

            packetType = $" -x{opCode:X2} - Unknown";
        }

        var direction = isIncoming ? " <- " : " -> ";
        using var sw = new StreamWriter(packetsDirectoryPath, true);
        sw.WriteLine(
            $"{DateTime.Now:HH:mm:ss.ffff}: {sessionId} {packetType}  {(direction)} (isCompress: {isCompressed}) 0x{buffer[0]:X2} (Length: {buffer.Length})"
        );
        sw.FormatBuffer(buffer);

        sw.WriteLine();
    }

    private void RegisterPackets()
    {
        _packetManager.RegisterPacket<ClientVersionRequest>();
        _packetManager.RegisterPacket<LoginRequest>();
        _packetManager.RegisterPacket<ConnectToGameServer>();
        _packetManager.RegisterPacket<SelectServer>();
        _packetManager.RegisterPacket<GameServerList>();
        _packetManager.RegisterPacket<LoginDenied>();
        _packetManager.RegisterPacket<GameServerLogin>();
        _packetManager.RegisterPacket<PingRequest>();
        _packetManager.RegisterPacket<CharacterCreation>();
    }

    public void Dispose()
    {
        _messageCancellationTokenSource.Dispose();
        _channelObservableSubscription?.Dispose();

        GC.SuppressFinalize(this);
    }

    public static IEnumerable<IPEndPoint> GetListeningAddresses(IPEndPoint ipep) =>
        NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(adapter =>
                adapter.GetIPProperties()
                    .UnicastAddresses
                    .Where(uip => ipep.AddressFamily == uip.Address.AddressFamily)
                    .Select(uip => new IPEndPoint(uip.Address, ipep.Port))
            );
}

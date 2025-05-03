using System.Net;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Packets.Entries;
using Prima.Network.Types;


namespace Prima.Server.Handlers;

public class LoginHandler
    : BasePacketListenerHandler, INetworkPacketListener<LoginRequest>, INetworkPacketListener<SelectServer>
{
    private readonly IAccountManager _accountManager;

    private readonly PrimaServerConfig _primaServerConfig;

    private readonly List<GameServerEntry> _gameServerEntries = new();

    public LoginHandler(
        ILogger<LoginHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IAccountManager accountManager, PrimaServerConfig primaServerConfig
    ) :
        base(logger, networkService, serviceProvider)
    {
        _accountManager = accountManager;
        _primaServerConfig = primaServerConfig;

        CreateGameServerList();
    }

    private void CreateGameServerList()
    {
        _gameServerEntries.Add(
            new GameServerEntry()
            {
                IP = IPAddress.Parse("127.0.0.1"),
                LoadPercent = 0x0,
                Name = _primaServerConfig.Shard.Name,
                TimeZone = 0
            }
        );
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<LoginRequest>(this);
        RegisterHandler<SelectServer>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, LoginRequest packet)
    {
        var login = await _accountManager.LoginGameAsync(packet.Username, packet.Password);

        if (login == null)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.IncorrectPassword));
            return;
        }

        if (!login.IsActive)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.AccountBlocked));
            return;
        }

        if (!login.IsVerified)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.CredentialsInvalid));
            return;
        }

        Logger.LogInformation("Login successful for user {Username}", login.Username);

        var gameServerList = new GameServerList();
        gameServerList.Servers.AddRange(_gameServerEntries);


        session.AccountId = login.Id.ToString();


        await session.SendPacketAsync(gameServerList);
    }

    public async Task OnPacketReceived(NetworkSession session, SelectServer packet)
    {
        Logger.LogInformation(
            "User selected server {ServerId}",
            packet.ShardId
        );

        var sessionKey = GenerateSessionKey();


        var gameServer = _gameServerEntries[packet.ShardId];

        var connectToServer = new ConnectToGameServer()
        {
            GameServerIP = gameServer.IP,
            GameServerPort = (ushort)_primaServerConfig.TcpServer.GamePort,
            SessionKey = sessionKey
        };


        await NetworkService.SendPacket(session.Id, connectToServer);
        // await session.SendPacketAsync(connectToServer);

        //await session.Disconnect();
    }

    public static uint GenerateSessionKey()
    {
        byte[] keyBytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }

        return BitConverter.ToUInt32(keyBytes, 0);
    }
}

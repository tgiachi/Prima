using System.Net;
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

    private List<GameServerEntry> _gameServerEntries = new();

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


        await session.SendPacketAsync(gameServerList);
    }

    public async Task OnPacketReceived(NetworkSession session, SelectServer packet)
    {
        session.AuthId = (uint)Random.Shared.Next();
        Logger.LogInformation(
            "User selected server {ServerId} generated authKey: {AuthKey}",
            packet.ShardId,
            session.AuthId
        );


        var gameServer = _gameServerEntries[packet.ShardId];

        await session.SendPacketAsync(
            new ConnectToGameServer()
            {
                GameServerIP = gameServer.IP,
                GameServerPort = (ushort)_primaServerConfig.TcpServer.GamePort,
                AuthKey = session.AuthId,
            }
        );
    }
}

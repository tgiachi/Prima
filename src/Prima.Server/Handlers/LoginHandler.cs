using System.Net;
using Microsoft.Extensions.Logging;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Packets.Entries;
using Prima.Network.Types;


namespace Prima.Server.Handlers;

public class LoginHandler : BasePacketListenerHandler, INetworkPacketListener<LoginRequest>
{
    private readonly IAccountManager _accountManager;

    public LoginHandler(
        ILogger<LoginHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IAccountManager accountManager
    ) :
        base(logger, networkService, serviceProvider)
    {
        _accountManager = accountManager;
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<LoginRequest>(this);
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

        gameServerList.Servers.Add(new GameServerEntry()
        {
            Index = 0,
            IP = IPAddress.Parse("127.0.0.1"),
            LoadPercent = 0x0,
            Name = "Test Server",
            TimeZone = 0
        });

        await session.SendPacketAsync(gameServerList);
    }
}

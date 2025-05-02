using Microsoft.Extensions.Logging;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
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
        }

        if (!login.IsActive)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.AccountBlocked));
        }
    }
}

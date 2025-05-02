using Microsoft.Extensions.Logging;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;


namespace Prima.Server.Handlers;

public class LoginHandler : BasePacketListenerHandler, INetworkPacketListener<LoginRequest>
{
    public LoginHandler(ILogger<LoginHandler> logger, INetworkService networkService, IServiceProvider serviceProvider) :
        base(logger, networkService, serviceProvider)
    {
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<LoginRequest>(this);
    }

    public async Task OnPacketReceived(string sessionId, LoginRequest packet)
    {
    }
}

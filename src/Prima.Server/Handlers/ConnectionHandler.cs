using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Server.Handlers;

public class ConnectionHandler : BasePacketListenerHandler, INetworkPacketListener<ClientVersion>
{
    public ConnectionHandler(
        ILogger<ConnectionHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    ) : base(logger, networkService, serviceProvider)
    {
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<ClientVersion>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, ClientVersion packet)
    {
        session.Seed = packet.Seed;
        session.ClientVersion = packet;

        session.FirstPacketReceived = true;
    }
}

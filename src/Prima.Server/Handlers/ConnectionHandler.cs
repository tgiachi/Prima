using Prima.Core.Server.Data;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Server.Handlers;

public class ConnectionHandler : BasePacketListenerHandler, INetworkPacketListener<ClientVersionRequest>
{
    public ConnectionHandler(
        ILogger<ConnectionHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    ) : base(logger, networkService, serviceProvider)
    {
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<ClientVersionRequest>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, ClientVersionRequest packet)
    {
        session.Seed = packet.Seed;
        session.ClientVersion = new ClientVersion(
            packet.MajorVersion,
            packet.MinorVersion,
            packet.Revision,
            packet.Prototype
        );

        if (PrimaServerContext.ClientVersion != session.ClientVersion)
        {
            Logger.LogWarning(
                "Client version mismatch. Expected: {@expected}, Received: {@received}",
                PrimaServerContext.ClientVersion,
                session.ClientVersion
            );
            await session.Disconnect();
            return;
        }


        session.FirstPacketReceived = true;
    }
}

using Orion.Network.Core.Extensions;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;

namespace Prima.Server.Handlers;

public class PingPongHandler : BasePacketListenerHandler, INetworkPacketListener<PingRequest>
{
    public PingPongHandler(
        ILogger<BasePacketListenerHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    ) : base(logger, networkService, serviceProvider)
    {
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<PingRequest>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, PingRequest request)
    {
        Logger.LogInformation(
            "Received Ping from SessionId:{Session}: {Sequence}",
            session.Id.ToShortSessionId(),
            request.Sequence
        );

        session.LastPing = DateTime.Now;

        await session.SendPacketAsync(request);
    }
}

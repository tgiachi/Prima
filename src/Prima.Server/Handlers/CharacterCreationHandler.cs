using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.UOData.Packets;

namespace Prima.Server.Handlers;

public class CharacterCreationHandler : BasePacketListenerHandler, INetworkPacketListener<CharacterCreation>
{
    public CharacterCreationHandler(
        ILogger<CharacterCreationHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    ) : base(logger, networkService, serviceProvider)
    {
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<CharacterCreation>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, CharacterCreation packet)
    {
        await session.SendPacketAsync(new ClientVersionReq());
    }
}

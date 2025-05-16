using Orion.Core.Server.Listeners.EventBus;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Types;
using Prima.UOData.Entities;
using Prima.UOData.Events.Login;
using Prima.UOData.Packets;
using Prima.UOData.Types;

namespace Prima.Server.Handlers;

public class LoginCompleteHandler : BasePacketListenerHandler, IEventBusListener<LoginCompleteEvent>
{
    public LoginCompleteHandler(
        ILogger<BasePacketListenerHandler> logger, INetworkService networkService, IServiceProvider serviceProvider
    ) : base(logger, networkService, serviceProvider)
    {
    }


    protected override void RegisterHandlers()
    {
        SubscribeEvent(this);
    }

    public async Task HandleAsync(LoginCompleteEvent @event, CancellationToken cancellationToken = default)
    {
        var session = SessionService.GetSession(@event.SessionId);
        var mobile = session.GetProperty<MobileEntity>();

        // Login confirmation packet
        await session.SendPacketAsync(new CharLocaleAndBody(mobile));
        // GeneralInformation packet
        await session.SendPacketAsync(new SeasonalInformation(Season.Spring, true));
        await session.SendPacketAsync(new DrawGamePlayer(mobile));
        await session.SendPacketAsync(new CharacterDraw(mobile));

        await session.SendPacketAsync(new GlobalLightLevel(0xFF));
        await session.SendPacketAsync(new PersonalLightLevel(mobile, 0xFF));
        await session.SendPacketAsync(new FeatureFlagsResponse(FeatureFlags.UOR | FeatureFlags.AOS));
        await session.SendPacketAsync(new CharacterWarMode(false));
        await session.SendPacketAsync(new LoginComplete());
    }
}

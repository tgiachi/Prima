using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Server.Modules.Scripts;
using Prima.UOData.Data.EventData;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Packets;

namespace Prima.Server.Handlers;

public class CharacterCreationHandler : BasePacketListenerHandler, INetworkPacketListener<CharacterCreation>
{
    private readonly IScriptEngineService _scriptEngineService;

    private readonly IMapService _mapService;

    public CharacterCreationHandler(
        ILogger<CharacterCreationHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IScriptEngineService scriptEngineService, IMapService mapService
    ) : base(logger, networkService, serviceProvider)
    {
        _scriptEngineService = scriptEngineService;
        _mapService = mapService;
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<CharacterCreation>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, CharacterCreation packet)
    {
        // TODO: persist character creation data

        TriggerCharacterCreatedEvent(packet);

        await session.SendPacketAsync(new ClientVersionReq());
    }

    private void TriggerCharacterCreatedEvent(CharacterCreation packet)
    {
        var eventArgs = new CharacterCreatedEventArgs(
            packet.Name,
            packet.IsFemale,
            packet.Hue,
            packet.Int,
            packet.Str,
            packet.Dex,
            _mapService.GetAvailableStartingCities()[packet.StartingLocation],
            packet.Skills,
            packet.ShirtColor,
            packet.PantsColor,
            packet.HairStyle,
            packet.HairColor,
            packet.FacialHair,
            packet.FacialHairColor,
            packet.Profession,
            packet.Race
        );

        _scriptEngineService.ExecuteCallback(nameof(EventScriptModule.OnCharacterCreated), eventArgs);
    }
}

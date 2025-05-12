using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Server.Modules.Scripts;
using Prima.UOData.Data.EventData;
using Prima.UOData.Packets;

namespace Prima.Server.Handlers;

public class CharacterCreationHandler : BasePacketListenerHandler, INetworkPacketListener<CharacterCreation>
{
    private readonly IScriptEngineService _scriptEngineService;

    public CharacterCreationHandler(
        ILogger<CharacterCreationHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IScriptEngineService scriptEngineService
    ) : base(logger, networkService, serviceProvider)
    {
        _scriptEngineService = scriptEngineService;
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
            null,
            packet.Skills,
            (int)packet.ShirtColor,
            (int)packet.PantsColor,
            (int)packet
                .HairStyle,
            (int)packet.HairColor,
            (int)packet.FacialHair,
            (int)packet.FacialHairColor,
            packet.Profession,
            packet.Race
        );

        _scriptEngineService.ExecuteCallback(nameof(EventScriptModule.OnCharacterCreated), eventArgs);
    }
}

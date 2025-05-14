using LiteDB;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Server.Modules.Scripts;
using Prima.UOData.Data.EventData;
using Prima.UOData.Entities;
using Prima.UOData.Entities.Db;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Packets;

namespace Prima.Server.Handlers;

public class CharacterHandler
    : BasePacketListenerHandler, INetworkPacketListener<CharacterCreation>, INetworkPacketListener<CharacterLogin>,
        INetworkPacketListener<CharacterDelete>
{
    private readonly IScriptEngineService _scriptEngineService;

    private readonly IWorldManagerService _worldManagerService;
    private readonly IDatabaseService _databaseService;

    private readonly IMapService _mapService;

    public CharacterHandler(
        ILogger<CharacterHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IScriptEngineService scriptEngineService, IMapService mapService, IWorldManagerService worldManagerService,
        IDatabaseService databaseService
    ) : base(logger, networkService, serviceProvider)
    {
        _scriptEngineService = scriptEngineService;
        _mapService = mapService;
        _worldManagerService = worldManagerService;
        _databaseService = databaseService;
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<CharacterCreation>(this);
        RegisterHandler<CharacterLogin>(this);
        RegisterHandler<CharacterDelete>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, CharacterCreation packet)
    {
        var characterEntity = new CharacterEntity
        {
            AccountId = new ObjectId(session.AccountId),
            Slot = packet.Slot
        };

        var playerMobile = _worldManagerService.GenerateWorldEntity<MobileEntity>();

        characterEntity.MobileId = playerMobile.Id;

        playerMobile.Name = packet.Name;

        _worldManagerService.AddWorldEntity(playerMobile);

        await _databaseService.InsertAsync(characterEntity);

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

    public async Task OnPacketReceived(NetworkSession session, CharacterLogin packet)
    {
        Logger.LogInformation("Character login request for {Name}", packet.Name);

        var character = await _databaseService.FirstOrDefaultAsync<CharacterEntity>(x =>
            x.AccountId == new ObjectId(session.AccountId) && x.Slot == packet.Slot
        );

        // TODO: Check if character exists

        session.SetProperty(character.MobileId, "mobileId");

        var mobile = _worldManagerService.GetEntityBySerial<MobileEntity>(character.MobileId);

        await session.SendPacketAsync(new ClientVersionReq());
    }

    public async Task OnPacketReceived(NetworkSession session, CharacterDelete packet)
    {
        Logger.LogInformation("Character delete request for slot {Name}", packet.Slot);
        var character = await _databaseService.FirstOrDefaultAsync<CharacterEntity>(x =>
            x.AccountId == new ObjectId(session.AccountId) && x.Slot == packet.Slot
        );

        if (character == null)
        {
            Logger.LogWarning("Character not found for slot {Slot}", packet.Slot);
            return;
        }

        var mobile = _worldManagerService.GetEntityBySerial<MobileEntity>(character.MobileId);

        if (mobile != null)
        {
            _worldManagerService.RemoveWorldEntity(mobile);
        }

        await _databaseService.DeleteAsync(character);
    }
}

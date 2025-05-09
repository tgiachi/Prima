using System.Net;
using System.Security.Cryptography;
using Orion.Foundations.Extensions;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Session;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Handlers.Base;
using Prima.Core.Server.Interfaces.Listeners;
using Prima.Core.Server.Interfaces.Services;
using Prima.Network.Packets;
using Prima.Network.Packets.Entries;
using Prima.Network.Types;
using Prima.UOData.Context;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Packets;


namespace Prima.Server.Handlers;

public class LoginHandler
    : BasePacketListenerHandler, INetworkPacketListener<LoginRequest>, INetworkPacketListener<SelectServer>,
        INetworkPacketListener<GameServerLogin>
{
    private readonly IAccountManager _accountManager;

    private readonly INetworkService _networkService;
    private readonly PrimaServerConfig _primaServerConfig;

    private readonly List<GameServerEntry> _gameServerEntries = new();

    private readonly IMapService _mapService;


    public LoginHandler(
        ILogger<LoginHandler> logger, INetworkService networkService, IServiceProvider serviceProvider,
        IAccountManager accountManager, PrimaServerConfig primaServerConfig, IMapService mapService
    ) :
        base(logger, networkService, serviceProvider)
    {
        _networkService = networkService;
        _accountManager = accountManager;
        _primaServerConfig = primaServerConfig;
        _mapService = mapService;

        CreateGameServerList();
    }

    private void CreateGameServerList()
    {
        _gameServerEntries.Add(
            new GameServerEntry()
            {
                IP = IPAddress.Parse("127.0.0.1"),
                LoadPercent = 0x0,
                Name = _primaServerConfig.Shard.Name,
                TimeZone = 2
            }
        );
    }

    protected override void RegisterHandlers()
    {
        RegisterHandler<LoginRequest>(this);
        RegisterHandler<SelectServer>(this);
        RegisterHandler<GameServerLogin>(this);
    }

    public async Task OnPacketReceived(NetworkSession session, LoginRequest packet)
    {
        if (session.Seed == 0)
        {
            Logger.LogWarning("User {SessionId} tried to login without a valid seed", session.Id);
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.CommunicationProblem));
            return;
        }

        var login = await _accountManager.LoginGameAsync(packet.Username, packet.Password);

        if (login == null)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.IncorrectPassword));
            return;
        }

        if (!login.IsActive)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.AccountBlocked));
            return;
        }

        if (!login.IsVerified)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.CredentialsInvalid));
            return;
        }

        Logger.LogInformation("Login successful for user {Username}", login.Username);

        var gameServerList = new GameServerList();
        gameServerList.Servers.AddRange(_gameServerEntries);


        session.AccountId = login.Id.ToString();


        await session.SendPacketAsync(gameServerList);
    }

    public async Task OnPacketReceived(NetworkSession session, SelectServer packet)
    {
        Logger.LogInformation(
            "User selected server {ServerId}",
            packet.ShardId
        );

        var sessionKey = GenerateSessionKey();

        session.AuthId = sessionKey;

        var gameServer = _gameServerEntries[packet.ShardId];


        /// 05/05/2025 --> Fixed connection bug after 4 days of testing, now i can die in peace! :D
        var connectToServer = new ConnectToGameServer()
        {
            GameServerIP = gameServer.IP,
            GameServerPort = (ushort)_primaServerConfig.TcpServer.GamePort,
            SessionKey = (int)sessionKey
        };

        Logger.LogDebug("Session generated is: {Session}", BitConverter.GetBytes(sessionKey).HumanizedContent());


        await session.SendPacketAsync(connectToServer);
    }

    public static uint GenerateSessionKey()
    {
        var randomNumber = RandomNumberGenerator.GetInt32(0, int.MaxValue);

        if (Random.Shared.Next(2) == 0)
        {
            randomNumber |= 1 << 31;
        }

        return (uint)randomNumber;
    }

    public async Task OnPacketReceived(NetworkSession session, GameServerLogin packet)
    {
        var account = await _accountManager.LoginGameAsync(packet.AccountId, packet.Password);

        if (account == null)
        {
            await session.SendPacketAsync(new LoginDenied(LoginDeniedReasonType.IncorrectPassword));
            await session.Disconnect();

            return;
        }

        session.UseNetworkCompression = true;

        _networkService.MoveLoginSessionToGameSession(session.Id, packet.SessionKey);

        // TODO: Check characeters on database

        var charactersAndCities = new CharactersStartingLocations(session.ClientVersion.ProtocolChanges);

        charactersAndCities.Cities.AddRange(_mapService.GetAvailableStartingCities());
        charactersAndCities.FillCharacters();


        await session.SendPacketAsync(charactersAndCities);

        await session.SendPacketAsync(new FeatureFlagsResponse(UOContext.ExpansionInfo.SupportedFeatures));
    }
}

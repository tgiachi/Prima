using System.Buffers.Binary;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Utils;
using Prima.Core.Server.Data;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Types;
using Prima.Core.Server.Types.Uo;
using Prima.UOData.Context;
using Prima.UOData.Data;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Mul;

namespace Prima.UOData.Services;

public class ClientVersionService : IClientVersionService, IEventBusListener<ServerStartedEvent>
{
    private readonly ILogger _logger;

    private readonly string _expansionConfigurationPath = "expansion.json";
    private readonly string _expansionsPath = "expansions.json";

    private readonly IEventBusService _eventBusService;
    private readonly PrimaServerConfig _primaServerConfig;
    private readonly DirectoriesConfig _directoriesConfig;

    public ClientVersionService(
        ILogger<ClientVersionService> logger, IEventBusService eventBusService, PrimaServerConfig primaServerConfig,
        DirectoriesConfig directoriesConfig
    )
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _primaServerConfig = primaServerConfig;
        _directoriesConfig = directoriesConfig;
        _expansionsPath = Path.Combine(_directoriesConfig[DirectoryType.Data], _expansionsPath);
        _expansionConfigurationPath = Path.Combine(_directoriesConfig[DirectoryType.Configs], _expansionConfigurationPath);

        _eventBusService.Subscribe(this);
    }

    public async Task HandleAsync(ServerStartedEvent @event, CancellationToken cancellationToken)
    {
        await GetClientVersionAsync();
        await GetExpansionAsync();
    }

    private async Task GetExpansionAsync()
    {
        ExpansionInfo.Table = JsonUtils.DeserializeFromFile<ExpansionInfo[]>(_expansionsPath);
        var expansion = JsonUtils.DeserializeFromFile<ExpansionInfo>(_expansionConfigurationPath);

        if (expansion == null)
        {
            UOContext.Expansion = Expansion.None;
        }


        var currentExpansionIndex = expansion.Id;
        ExpansionInfo.Table[currentExpansionIndex] = expansion;
        UOContext.Expansion = (Expansion)currentExpansionIndex;


    }

    private async Task GetClientVersionAsync()
    {
        ClientVersion clientVersion = null;
        _logger.LogInformation("Determining client version...");

        if (!string.IsNullOrEmpty(_primaServerConfig.Shard.ClientVersion))
        {
            _logger.LogInformation("Client version set to {@clientVersion}", _primaServerConfig.Shard.ClientVersion);

            clientVersion = new ClientVersion(_primaServerConfig.Shard.ClientVersion);

            if (clientVersion == null)
            {
                _logger.LogError("Invalid client version format: {@clientVersion}", _primaServerConfig.Shard.ClientVersion);
                throw new ArgumentException("Invalid client version format");
            }
        }

        var uoClassic = UoFiles.GetFilePath("client.exe");

        if (!string.IsNullOrEmpty(uoClassic))
        {
            await using FileStream fs = new FileStream(uoClassic, FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = GC.AllocateUninitializedArray<byte>((int)fs.Length, true);
            _ = fs.Read(buffer);
            // VS_VERSION_INFO (unicode)
            Span<byte> vsVersionInfo =
            [
                0x56, 0x00, 0x53, 0x00, 0x5F, 0x00, 0x56, 0x00,
                0x45, 0x00, 0x52, 0x00, 0x53, 0x00, 0x49, 0x00,
                0x4F, 0x00, 0x4E, 0x00, 0x5F, 0x00, 0x49, 0x00,
                0x4E, 0x00, 0x46, 0x00, 0x4F, 0x00
            ];

            var versionIndex = buffer.AsSpan().IndexOf(vsVersionInfo);
            if (versionIndex > -1)
            {
                var offset = versionIndex + 42; // 30 + 12

                var minorPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset));
                var majorPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 2));
                var privatePart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 4));
                var buildPart = BinaryPrimitives.ReadUInt16LittleEndian(buffer.AsSpan(offset + 6));

                clientVersion = new ClientVersion(majorPart, minorPart, buildPart, privatePart);
            }
        }

        if (clientVersion == null)
        {
            _logger.LogError("Client version not found");
            throw new InvalidOperationException("Client version not found");
        }

        UOContext.ClientVersion = clientVersion;
    }
}

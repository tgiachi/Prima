using System.Buffers.Binary;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Extensions;
using Orion.Foundations.Utils;
using Prima.Core.Server.Data;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Types;
using Prima.Core.Server.Types.Uo;
using Prima.UOData.Context;
using Prima.UOData.Data;
using Prima.UOData.Data.Tiles;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Mul;
using Prima.UOData.Types;
using Prima.UOData.Utils;

namespace Prima.UOData.Services;

public class ClientConfigurationService : IClientConfigurationService, IEventBusListener<ServerStartedEvent>
{
    private readonly ILogger _logger;

    private readonly string _expansionConfigurationPath = "expansion.json";
    private readonly string _expansionsPath = "expansions.json";

    private readonly IEventBusService _eventBusService;
    private readonly PrimaServerConfig _primaServerConfig;
    private readonly DirectoriesConfig _directoriesConfig;

    public ClientConfigurationService(
        ILogger<ClientConfigurationService> logger, IEventBusService eventBusService, PrimaServerConfig primaServerConfig,
        DirectoriesConfig directoriesConfig
    )
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _primaServerConfig = primaServerConfig;
        _directoriesConfig = directoriesConfig;
        _expansionsPath = Path.Combine(_directoriesConfig[DirectoryType.Data], _expansionsPath);
        _expansionConfigurationPath = Path.Combine(_directoriesConfig[DirectoryType.Configs], _expansionConfigurationPath);

        LoadData();
        _eventBusService.Subscribe(this);
    }

    private async Task LoadData()
    {
        var startTime = Stopwatch.GetTimestamp();
        UoFiles.ScanForFiles(_primaServerConfig.Shard.UoDirectory);

        await GetClientVersionAsync();
        await GetExpansionAsync();
        await LoadSkillInfoAsync();
        await BuildProfessionsAsync();


        _logger.LogInformation("Loading TileData...");
        TileData.Configure();

        _logger.LogInformation("Found {Count} Items", TileData.ItemTable.Length);
        _logger.LogInformation("Found {Count} LandTiles", TileData.LandTable.Length);

        RaceDefinitions.Configure();

        MultiData.Configure();

        _logger.LogInformation("Found {Count} MultiDefs", MultiData.Count);

        _logger.LogInformation("Loading took {Elapsed}ms", Stopwatch.GetElapsedTime(startTime));
    }

    public async Task HandleAsync(ServerStartedEvent @event, CancellationToken cancellationToken)
    {
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
        UOContext.ExpansionInfo = expansion;
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


    private async Task LoadSkillInfoAsync()
    {
        SkillInfo.Table = (await File.ReadAllTextAsync(Path.Combine(_directoriesConfig[DirectoryType.Data], "skills.json")))
            .FromJson<SkillInfo[]>();
    }

    private async Task BuildProfessionsAsync()
    {
        var path = Path.Combine(_directoriesConfig[DirectoryType.Configs], "prof.txt");
        if (!File.Exists(path))
        {
            var parent = Path.Combine(_directoriesConfig[DirectoryType.Configs], "Professions");

            path = Path.Combine(ExpansionInfo.GetEraFolder(parent), "prof.txt");
        }

        if (File.Exists(path))
        {
            var maxProf = 0;
            List<ProfessionInfo> profs = [];

            using var s = File.OpenText(path);

            while (!s.EndOfStream)
            {
                var line = s.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                line = line.Trim();

                if (!line.InsensitiveStartsWith("Begin"))
                {
                    continue;
                }

                var prof = new ProfessionInfo();

                var totalStats = 0;
                var skillIndex = 0;
                var totalSkill = 0;

                while (!s.EndOfStream)
                {
                    line = await s.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    line = line.Trim();

                    if (line.InsensitiveStartsWith("End"))
                    {
                        if (prof.ID > 0 && totalStats >= 80 && totalSkill >= 100)
                        {
                            prof.FixSkills(); // Adjust skills array in case there are fewer skills than the default 4
                            profs.Add(prof);
                        }

                        break;
                    }

                    var cols = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                    var key = cols[0].ToLowerInvariant();
                    var value = cols[1].Trim('"');

                    if (key == "type" && !value.InsensitiveEquals("profession"))
                    {
                        break;
                    }

                    switch (key)
                    {
                        case "truename":
                            {
                                prof.Name = value;
                            }
                            break;
                        case "nameid":
                            {
                                prof.NameID = Utility.ToInt32(value);
                                break;
                            }
                        case "descid":
                            {
                                prof.DescID = Utility.ToInt32(value);
                                break;
                            }
                        case "desc":
                            {
                                prof.ID = Utility.ToInt32(value);
                                if (prof.ID > maxProf)
                                {
                                    maxProf = prof.ID;
                                }
                            }
                            break;
                        case "toplevel":
                            {
                                prof.TopLevel = Utility.ToBoolean(value);
                                break;
                            }
                        case "gump":
                            {
                                prof.GumpID = Utility.ToInt32(value);
                                break;
                            }
                        case "skill":
                            {
                                if (!ProfessionInfo.TryGetSkillName(value, out var skillName))
                                {
                                    break;
                                }

                                var skillValue = byte.Parse(cols[2]);
                                prof.Skills[skillIndex++] = (skillName, skillValue);
                                totalSkill += skillValue;
                            }
                            break;
                        case "stat":
                            {
                                if (!Enum.TryParse(value, out StatType stat))
                                {
                                    break;
                                }

                                var statValue = byte.Parse(cols[2]);
                                prof.Stats[(int)stat >> 1] = statValue;
                                totalStats += statValue;
                            }
                            break;
                    }
                }
            }


            ProfessionInfo.Professions = new ProfessionInfo[maxProf + 1];

            foreach (var p in profs)
            {
                ProfessionInfo.Professions[p.ID] = p;
            }

            profs.Clear();
            profs.TrimExcess();
        }
        else
        {
            ProfessionInfo.Professions = new ProfessionInfo[1];
        }

        ProfessionInfo.Professions[0] = new ProfessionInfo
        {
            Name = "Advanced Skills"
        };
    }
}

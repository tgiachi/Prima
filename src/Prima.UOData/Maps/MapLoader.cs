using System.Diagnostics;
using System.Text.Json;
using Serilog;

namespace Prima.UOData.Maps;


public static class MapLoader
{
    private static readonly ILogger logger = Log.Logger.ForContext(typeof(MapLoader));

    /* Here we configure all maps. Some notes:
     *
     * 1) The first 32 maps are reserved for core use.
     * 2) Map 127 is reserved for core use.
     * 3) Map 255 is reserved for core use.
     * 4) Changing or removing any predefined maps may cause server instability.
     *
     * Map definitions are modified in Data/Map Definitions/<expansion>.json:
     *  - <index> : An unreserved unique index for this map
     *  - <id> : An identification number used in client communications. For any visible maps, this value must be from 0-5
     *  - <fileIndex> : A file identification number. For any visible maps, this value must be from 0-5
     *  - <width>, <height> : Size of the map (in tiles)
     *  - <season> : Season of the map. 0 = Spring, 1 = Summer, 2 = Fall, 3 = Winter, 4 = Desolation
     *  - <name> : Reference name for the map, used in props gump, get/set commands, region loading, etc
     *  - <rules> : Rules and restrictions associated with the map. See documentation for details
     */
    // public static void Configure()
    // {
    //     var failures = new List<string>();
    //     var count = 0;
    //
    //     var path = Path.Combine(Core.BaseDirectory, "Data/map-definitions.json");
    //
    //     logger.Information("Loading Map Definitions");
    //
    //     var stopwatch = Stopwatch.StartNew();
    //     var maps = JsonConfig.Deserialize<List<MapDefinition>>(path);
    //     if (maps == null)
    //     {
    //         throw new JsonException($"Failed to deserialize {path}.");
    //     }
    //
    //     foreach (var def in maps)
    //     {
    //         try
    //         {
    //             RegisterMap(def);
    //             count++;
    //         }
    //         catch (Exception ex)
    //         {
    //             logger.Debug(ex, "Failed to load map definition {MapDefName} ({MapDefId})", def.Name, def.Id);
    //             failures.Add($"\tInvalid map definition {def.Name} ({def.Id})");
    //         }
    //     }
    //
    //     stopwatch.Stop();
    //
    //     if (failures.Count > 0)
    //     {
    //         logger.Warning(
    //             $"Loading map definitions {{Status}} ({{Count}} maps, {{FailureCount}} failures) ({{Duration:F2}} seconds){Environment.NewLine}{{Failures}}",
    //             "failed",
    //             count,
    //             failures.Count,
    //             stopwatch.Elapsed.TotalSeconds,
    //             failures
    //         );
    //     }
    //     else
    //     {
    //         logger.Information(
    //             "Loading map definitions {Status} ({Count} maps, {FailureCount} failures) ({Duration:F2} seconds)",
    //             "done",
    //             count,
    //             failures.Count,
    //             stopwatch.Elapsed.TotalSeconds
    //         );
    //     }
    // }

    // private static void RegisterMap(MapDefinition mapDefinition)
    // {
    //     var newMap = new Map(
    //         mapDefinition.Id,
    //         mapDefinition.Index,
    //         mapDefinition.FileIndex,
    //         Math.Max(mapDefinition.Width, Map.SectorSize),
    //         Math.Max(mapDefinition.Height, Map.SectorSize),
    //         mapDefinition.Season,
    //         mapDefinition.Name,
    //         mapDefinition.Rules
    //     );
    //
    //     Map.Maps[mapDefinition.Index] = newMap;
    //     Map.AllMaps.Add(newMap);
    // }
}

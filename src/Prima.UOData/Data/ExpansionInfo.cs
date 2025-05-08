using System.Text.Json;
using System.Text.Json.Serialization;
using Orion.Core.Server.Data.Directories;
using Orion.Foundations.Extensions;
using Prima.Core.Server.Converters.Json;
using Prima.Core.Server.Data.Uo;
using Prima.Core.Server.Types;
using Prima.Core.Server.Types.Uo;
using Prima.Network.Types;
using Prima.UOData.Context;
using Prima.UOData.Types;

namespace Prima.UOData.Data;

public class ExpansionInfo
{
    public static bool ForceOldAnimations { get; private set; }


    public static string GetEraFolder(string parentDirectory)
    {
        var expansion = UOContext.Expansion;
        var folders = Directory.GetDirectories(
            parentDirectory,
            "*",
            new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive }
        );

        while (expansion-- >= 0)
        {
            foreach (var folder in folders)
            {
                var di = new DirectoryInfo(folder);
                if (di.Name.InsensitiveEquals(expansion.ToString()))
                {
                    return folder;
                }
            }
        }

        return null;
    }

    public static void StoreMapSelection(MapSelectionFlags mapSelectionFlags, Expansion expansion)
    {
        int expansionIndex = (int)expansion;
        Table[expansionIndex].MapSelectionFlags = mapSelectionFlags;
    }


    // public static bool LoadConfiguration(DirectoriesConfig directories, out Expansion expansion)
    // {
    //     var path = Path.Combine(directories[DirectoryType.Data], "expansions.json");
    //
    //     if (!File.Exists(path))
    //     {
    //         throw new FileNotFoundException($"Expansion file '{path}' could not be found.");
    //     }
    //
    //     var pathToExpansions =
    //         File.ReadAllText(path);
    //
    //     Table = pathToExpansions.FromJson<ExpansionInfo[]>();
    //
    //     var pathToExpansionFile = Path.Combine(directories[DirectoryType.Configs], ExpansionConfigurationPath);
    //
    //
    //     ExpansionInfo expansionConfig = File.ReadAllText(pathToExpansionFile).FromJson<ExpansionInfo>();
    //     if (expansionConfig == null)
    //     {
    //         expansion = Expansion.None;
    //         return false;
    //     }
    //
    //     int currentExpansionIndex = expansionConfig.Id;
    //     Table[currentExpansionIndex] = expansionConfig;
    //     expansion = (Expansion)currentExpansionIndex;
    //     return true;
    // }


    public ExpansionInfo(
        int id,
        string name,
        ClientFlags clientFlags,
        FeatureFlags supportedFeatures,
        CharacterListFlags charListFlags,
        HousingFlags customHousingFlag,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    ) : this(id, name, supportedFeatures, charListFlags, customHousingFlag, mobileStatusVersion, mapSelectionFlags) =>
        ClientFlags = clientFlags;

    public ExpansionInfo(
        int id,
        string name,
        ClientVersion requiredClient,
        FeatureFlags supportedFeatures,
        CharacterListFlags charListFlags,
        HousingFlags customHousingFlag,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    ) : this(id, name, supportedFeatures, charListFlags, customHousingFlag, mobileStatusVersion, mapSelectionFlags) =>
        RequiredClient = requiredClient;

    [JsonConstructor]
    public ExpansionInfo(
        int id,
        string name,
        FeatureFlags supportedFeatures,
        CharacterListFlags characterListFlags,
        HousingFlags housingFlags,
        int mobileStatusVersion,
        MapSelectionFlags mapSelectionFlags
    )
    {
        Id = id;
        Name = name;

        SupportedFeatures = supportedFeatures;
        CharacterListFlags = characterListFlags;
        HousingFlags = housingFlags;
        MobileStatusVersion = mobileStatusVersion;
        MapSelectionFlags = mapSelectionFlags;
    }

    public static ExpansionInfo CoreExpansion => GetInfo(UOContext.Expansion);

    public static ExpansionInfo[] Table { get; set; }

    public int Id { get; }
    public string Name { get; set; }

    public ClientFlags ClientFlags { get; set; }

    [JsonConverter(typeof(FlagsConverter<FeatureFlags>))]
    public FeatureFlags SupportedFeatures { get; set; }

    [JsonConverter(typeof(FlagsConverter<CharacterListFlags>))]
    public CharacterListFlags CharacterListFlags { get; set; }

    public ClientVersion RequiredClient { get; set; }

    [JsonConverter(typeof(FlagsConverter<HousingFlags>))]
    public HousingFlags HousingFlags { get; set; }

    public int MobileStatusVersion { get; set; }

    [JsonConverter(typeof(FlagsConverter<MapSelectionFlags>))]
    public MapSelectionFlags MapSelectionFlags { get; set; }

    public static ExpansionInfo GetInfo(Expansion ex) => GetInfo((int)ex);

    public static ExpansionInfo GetInfo(int ex)
    {
        var v = ex;

        if (v < 0 || v >= Table.Length)
        {
            v = 0;
        }

        return Table[v];
    }

    public override string ToString() => Name;
}

using Microsoft.Extensions.Logging;
using Prima.UOData.Data.Map;
using Prima.UOData.Interfaces.Services;

namespace Prima.UOData.Services;

public class MapService : IMapService
{
    public static readonly CityInfo[] OldHavenStartingCities =
    [
        new("Haven", "The Bountiful Harvest Inn", 3677, 2625, 0, 0, 1),
        new("Britain", "Sweet Dreams Inn", 1075074, 1496, 1628, 10, 1),
        new("Magincia", "The Great Horns Tavern", 1075077, 3734, 2222, 20, 1),
    ];

    public static readonly CityInfo[] FeluccaStartingCities =
    [
        new("Yew", "The Empath Abbey", 1075072, 633, 858, 0, 0),
        new("Minoc", "The Barnacle", 1075073, 2476, 413, 15, 0),
        new("Britain", "Sweet Dreams Inn", 1075074, 1496, 1628, 10, 0),
        new("Moonglow", "The Scholars Inn", 1075075, 4408, 1168, 0, 0),
        new("Trinsic", "The Traveler's Inn", 1075076, 1845, 2745, 0, 0),
        new("Magincia", "The Great Horns Tavern", 1075077, 3734, 2222, 20, 0),
        new("Jhelom", "The Mercenary Inn", 1075078, 1374, 3826, 0, 0),
        new("Skara Brae", "The Falconer's Inn", 1075079, 618, 2234, 0, 0),
        new("Vesper", "The Ironwood Inn", 1075080, 2771, 976, 0, 0)
    ];

    public static readonly CityInfo[] TrammelStartingCities =
    [
        new("Yew", "The Empath Abbey", 1075072, 633, 858, 0, 1),
        new("Minoc", "The Barnacle", 1075073, 2476, 413, 15, 1),
        new("Moonglow", "The Scholars Inn", 1075075, 4408, 1168, 0, 1),
        new("Trinsic", "The Traveler's Inn", 1075076, 1845, 2745, 0, 1),
        new("Jhelom", "The Mercenary Inn", 1075078, 1374, 3826, 0, 1),
        new("Skara Brae", "The Falconer's Inn", 1075079, 618, 2234, 0, 1),
        new("Vesper", "The Ironwood Inn", 1075080, 2771, 976, 0, 1),
    ];

    public static readonly CityInfo[] NewHavenStartingCities =
    [
        new("New Haven", "The Bountiful Harvest Inn", 1150168, 3503, 2574, 14, 1),
        new("Britain", "The Wayfarer's Inn", 1075074, 1602, 1591, 20, 1)
        // Magincia removed because it burned down.
    ];

    public static readonly CityInfo[] StartingCitiesSA =
    [
        new("Royal City", "Royal City Inn", 1150169, 738, 3486, -19, 5)
    ];

    private readonly ILogger _logger;

    public MapService(ILogger<MapService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}

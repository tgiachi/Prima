using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Events.Server;
using Orion.Core.Server.Interfaces.Services.System;
using Orion.Core.Server.Listeners.EventBus;
using Orion.Foundations.Utils;
using Prima.Server.Interfaces.Services;

namespace Prima.Server.Services;

public class AssetService : IAssetService
{
    private readonly ILogger _logger;

    private readonly IEventBusService _eventBusService;

    private readonly DirectoriesConfig _directoriesConfig;

    public AssetService(ILogger<AssetService> logger, IEventBusService eventBusService, DirectoriesConfig directoriesConfig)
    {
        _logger = logger;
        _eventBusService = eventBusService;
        _directoriesConfig = directoriesConfig;


        CopyFilesAsync();
    }

    private async Task CopyFilesAsync()
    {
        var assets = ResourceUtils.GetEmbeddedResourceNames(typeof(AssetService).Assembly, "Assets");
        var files = assets.Select(s => new
                { Asset = s, FileName = ResourceUtils.ConvertResourceNameToPath(s, "Prima.Server.Assets") }
            )
            .ToList();


        foreach (var assetFile in files)
        {
            var fileName = Path.Combine(_directoriesConfig.Root, assetFile.FileName);

            if (!File.Exists(fileName))
            {
                _logger.LogInformation("Copying asset  {FileName}", fileName);

                var content = ResourceUtils.GetEmbeddedResourceContent(assetFile.Asset, typeof(AssetService).Assembly);

                var directory = Path.GetDirectoryName(fileName);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fileName, content);
            }
        }
    }
}

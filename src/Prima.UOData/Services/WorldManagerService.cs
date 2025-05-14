using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Interfaces.Services.System;
using Prima.Core.Server.Data.Config;
using Prima.Core.Server.Types;
using Prima.UOData.Entities;
using Prima.UOData.Events.World;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Entities;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Services;
using Prima.UOData.Serializers.Binary;

namespace Prima.UOData.Services;

public class WorldManagerService : IWorldManagerService
{
    private readonly ILogger _logger;
    private readonly IPersistenceManager _persistenceManager;

    private readonly IEventBusService _eventBusService;
    private readonly DirectoriesConfig _directoriesConfig;

    private readonly SemaphoreSlim _entitiesSemaphore = new(1, 1);

    private readonly SortedDictionary<Serial, MobileEntity> _mobiles = new();
    private readonly SortedDictionary<Serial, ItemEntity> _items = new();

    private Serial _lastMobileSerial;

    private Serial _lastItemSerial;

    private readonly ISchedulerSystemService _schedulerSystemService;

    private readonly PrimaServerConfig _primaServerConfig;

    public WorldManagerService(
        ILogger<WorldManagerService> logger, IPersistenceManager persistenceManager, DirectoriesConfig directoriesConfig,
        PrimaServerConfig primaServerConfig, ISchedulerSystemService schedulerSystemService, IEventBusService eventBusService
    )
    {
        _persistenceManager = persistenceManager;
        _directoriesConfig = directoriesConfig;
        _primaServerConfig = primaServerConfig;
        _schedulerSystemService = schedulerSystemService;
        _eventBusService = eventBusService;
        _logger = logger;

        _persistenceManager.RegisterEntitySerializer(new BinaryItemSerializer());
        _persistenceManager.RegisterEntitySerializer(new BinaryMobileSerializer());
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Autosave is enabled. Saving world data every {Interval} minutes.",
            _primaServerConfig.Shard.Autosave.IntervalInMinutes
        );
        _schedulerSystemService.RegisterJob(
            "world_save",
            SaveWorldAsync,
            TimeSpan.FromMinutes(_primaServerConfig.Shard.Autosave.IntervalInMinutes)
        );
    }

    public async Task SaveWorldAsync()
    {
        await _eventBusService.PublishAsync(new WorldSavingEvent());

        // Wait for the event to be processed
        await Task.Delay(1000);
        await _entitiesSemaphore.WaitAsync();

        var startTime = Stopwatch.GetTimestamp();
        _logger.LogInformation("Autosaving world data...");

        var items = _items.Values.ToList();

        var mobiles = _mobiles.Values.ToList();

        var saveDirectory = Path.Combine(_directoriesConfig[DirectoryType.WorldSaves], $"{DateTime.Now:yyyyMMddHHmmss}");
        Directory.CreateDirectory(saveDirectory);

        var itemsFileName = Path.Combine(saveDirectory, "items.bin");
        var mobilesFileName = Path.Combine(saveDirectory, "mobiles.bin");

        await _persistenceManager.SaveToFileAsync(items, itemsFileName);

        await _persistenceManager.SaveToFileAsync(mobiles, mobilesFileName);

        _logger.LogInformation("World saved in {Elapsed}", Stopwatch.GetElapsedTime(startTime));

        _entitiesSemaphore.Release();

        await _eventBusService.PublishAsync(
            new WorldSavedEvent(Stopwatch.GetElapsedTime(startTime), items.Count + mobiles.Count)
        );
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void AddWorldEntity(IHaveSerial entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _entitiesSemaphore.Wait();

        if (entity is MobileEntity mobile)
        {
            _mobiles.Add(mobile.Id, mobile);
            _lastMobileSerial = mobile.Id;
        }
        else if (entity is ItemEntity item)
        {
            _items.Add(item.Id, item);
            _lastItemSerial = item.Id;
        }

        _entitiesSemaphore.Release();
    }

    public TEntity GenerateWorldEntity<TEntity>() where TEntity : IHaveSerial
    {
        ArgumentNullException.ThrowIfNull(typeof(TEntity));

        _entitiesSemaphore.Wait();

        try
        {
            if (typeof(TEntity) == typeof(MobileEntity))
            {
                return (TEntity)(object)new MobileEntity(_lastMobileSerial++);
            }

            if (typeof(TEntity) == typeof(ItemEntity))
            {
                return (TEntity)(object)new ItemEntity(Serial.ItemOffsetSerial + _lastItemSerial++);
            }

            throw new InvalidOperationException($"Unknown entity type {typeof(TEntity)}");
        }
        finally
        {
            _entitiesSemaphore.Release();
        }
    }
}

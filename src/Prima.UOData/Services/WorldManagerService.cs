using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Orion.Core.Server.Events.Server;
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

        _eventBusService.Subscribe<ServerStoppingEvent>(OnServerStopping);

        await LoadLastWorldAsync();
    }

    private async Task OnServerStopping(ServerStoppingEvent arg)
    {
        await SaveWorldAsync();
    }


    public async Task LoadLastWorldAsync()
    {
        _items.Clear();
        _mobiles.Clear();

        var start = Stopwatch.GetTimestamp();
        await _entitiesSemaphore.WaitAsync();

        var allSavesDirectory = _directoriesConfig[DirectoryType.WorldSaves];
        var lastSaveDirectory = Directory.GetDirectories(allSavesDirectory)
            .OrderByDescending(d => d)
            .FirstOrDefault();

        if (lastSaveDirectory is null)
        {
            _logger.LogWarning("No world save found.");
            _entitiesSemaphore.Release();
            return;
        }

        var itemsFileName = Path.Combine(lastSaveDirectory, "items.bin");
        var mobilesFileName = Path.Combine(lastSaveDirectory, "mobiles.bin");

        if (File.Exists(itemsFileName))
        {
            var items = await _persistenceManager.DeserializeAsync<ItemEntity>(itemsFileName);
            foreach (var item in items)
            {
                _items.Add(item.Id, item);
            }
        }

        if (File.Exists(mobilesFileName))
        {
            var mobiles = await _persistenceManager.DeserializeAsync<MobileEntity>(mobilesFileName);
            foreach (var mobile in mobiles)
            {
                _mobiles.Add(mobile.Id, mobile);
            }
        }

        await _eventBusService.PublishAsync(
            new WorldLoadedEvent(
                Stopwatch.GetElapsedTime(start),
                _mobiles.Count + _items.Count
            )
        );

        _logger.LogInformation(
            "World loaded in {Elapsed}. Loaded {Count} entities.",
            Stopwatch.GetElapsedTime(start),
            _mobiles.Count + _items.Count
        );

        _entitiesSemaphore.Release();
    }

    public TEntity? GetEntityBySerial<TEntity>(Serial id) where TEntity : IHaveSerial
    {
        if (typeof(TEntity) == typeof(MobileEntity))
        {
            return _mobiles.TryGetValue(id, out var mobile) ? (TEntity)(object)mobile : default;
        }

        if (typeof(TEntity) == typeof(ItemEntity))
        {
            return _items.TryGetValue(id, out var item) ? (TEntity)(object)item : default;
        }

        throw new InvalidOperationException($"Unknown entity type {typeof(TEntity)}");
    }

    public bool RemoveWorldEntity(IHaveSerial entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _entitiesSemaphore.Wait();

        if (entity is MobileEntity mobile)
        {
            _mobiles.Remove(mobile.Id);
        }
        else if (entity is ItemEntity item)
        {
            _items.Remove(item.Id);
        }
        else
        {
            _entitiesSemaphore.Release();
            return false;
        }

        _entitiesSemaphore.Release();
        return true;
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

        if (items.Count == 0 && mobiles.Count == 0)
        {
            _logger.LogInformation("No world data to save.");
            _entitiesSemaphore.Release();
            return;
        }

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

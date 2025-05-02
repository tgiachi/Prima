using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Logging;
using Orion.Core.Server.Data.Directories;
using Prima.Core.Server.Interfaces.Entities;
using Prima.Core.Server.Interfaces.Services;
using Prima.Core.Server.Types;

namespace Prima.Server.Services;

public class LiteDbDatabaseService : IDatabaseService
{
    private readonly ILogger _logger;
    private readonly DirectoriesConfig _directoriesConfig;

    private readonly LiteDatabaseAsync _database;

    public LiteDbDatabaseService(ILogger<LiteDbDatabaseService> logger, DirectoriesConfig directoriesConfig)
    {
        _logger = logger;
        _directoriesConfig = directoriesConfig;

        var connectionString = new ConnectionString()
        {
            Filename = Path.Combine(
                directoriesConfig[DirectoryType.Database],
                "prima.db"
            ),
            Connection = ConnectionType.Shared,
        };
        _database = new LiteDatabaseAsync(connectionString);
    }


    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    public async Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity
    {
        await InsertAsync([entity]);
        return entity;
    }

    public async Task<List<TEntity>> InsertAsync<TEntity>(List<TEntity> entities) where TEntity : class, IPrimaDbEntity
    {
        var startTime = Stopwatch.GetTimestamp();
        var collection = _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity)));

        entities.ForEach(
            e =>
            {
                e.Id = ObjectId.NewObjectId();
                e.Created = DateTime.UtcNow;
                e.Updated = DateTime.UtcNow;
            }
        );


        await collection.InsertAsync(entities);

        var endTime = Stopwatch.GetTimestamp();


        _logger.LogDebug(
            "Inserted {Count} entities of type {Type} in {Time} ms",
            entities.Count,
            typeof(TEntity).Name,
            Stopwatch.GetElapsedTime(startTime, endTime)
        );
        return entities;
    }

    public async Task<int> CountAsync<TEntity>() where TEntity : class, IPrimaDbEntity
    {
        var startTime = Stopwatch.GetTimestamp();
        var count = await _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).CountAsync();

        var endTime = Stopwatch.GetTimestamp();

        _logger.LogDebug(
            "Counted {Count} entities of type {Type} in {Time} ms",
            count,
            typeof(TEntity).Name,
            Stopwatch.GetElapsedTime(startTime, endTime)
        );

        return count;
    }

    public Task<TEntity> FindByIdAsync<TEntity>(Guid id) where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).FindByIdAsync(id);
    }

    public async Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class, IPrimaDbEntity
    {
        var startTime = Stopwatch.GetTimestamp();
        var entities = (await _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).FindAllAsync()).ToList();

        var endTime = Stopwatch.GetTimestamp();

        _logger.LogDebug(
            "Found {Count} entities of type {Type} in {Time} ms",
            entities.Count(),
            typeof(TEntity).Name,
            Stopwatch.GetElapsedTime(startTime, endTime)
        );

        return entities;
    }

    public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).FindAsync(predicate);
    }

    public Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).FindOneAsync(predicate);
    }

    public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity
    {
        entity.Updated = DateTime.UtcNow;
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).UpdateAsync(entity);
    }

    public Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).DeleteAsync(entity.Id);
    }

    public Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).DeleteAsync(id);
    }

    public Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).DeleteManyAsync(predicate);
    }

    public Task DeleteAllAsync<TEntity>() where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).DeleteManyAsync(e => true);
    }

    public Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IPrimaDbEntity
    {
        return _database.GetCollection<TEntity>(GetCollectionName(typeof(TEntity))).ExistsAsync(predicate);
    }

    private static string GetCollectionName(Type type)
    {
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();

        return tableAttribute?.Name ?? type.Name;
    }
}

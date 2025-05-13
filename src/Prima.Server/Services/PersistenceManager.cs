using Orion.Core.Server.Data.Directories;
using Prima.Core.Server.Data.Serialization;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.Server.Services;

public class PersistenceManager : IPersistenceManager
{
    private readonly ILogger<PersistenceManager> _logger;

    private readonly Dictionary<Type, IEntitySerializer> _entitySerializers = new();

    private readonly DirectoriesConfig _directoriesConfig;

    public PersistenceManager(ILogger<PersistenceManager> logger, DirectoriesConfig directoriesConfig)
    {
        _logger = logger;
        _directoriesConfig = directoriesConfig;
    }


    public Task<List<TEntity>> DeserializeAsync<TEntity>(byte[] data) where TEntity : ISerializableEntity
    {
        throw new NotImplementedException();
    }

    public async Task<SerializationEntryData> SerializeAsync<TEntity>(TEntity entity) where TEntity : ISerializableEntity
    {
        if (!_entitySerializers.TryGetValue(typeof(TEntity), out var serializer))
        {
            throw new InvalidOperationException($"No serializer registered for entity type {typeof(TEntity)}");
        }

        var serializerData = serializer.Serialize(entity);

        return new SerializationEntryData(serializer.Header, serializerData, serializer.FileName);
    }

    public void RegisterEntitySerializer<TEntity>(IEntitySerializer<TEntity> serializer) where TEntity : ISerializableEntity
    {
        ArgumentNullException.ThrowIfNull(serializer);

        var type = typeof(TEntity);
        if (_entitySerializers.ContainsKey(type))
        {
            _logger.LogWarning("Serializer for entity type {Type} is already registered. Overwriting.", type);
        }

        _entitySerializers[serializer.EntityType] = serializer;
    }

    public async Task SaveToFileAsync(List<SerializationEntryData> entries, string fileName)
    {
        await using var stream = new FileStream(fileName, FileMode.Create | FileMode.Open, FileAccess.Write, FileShare.None);
        await using var writer = new BinaryWriter(stream);

        writer.Write("PRIMA"u8.ToArray()); // Magic number
        writer.Write((byte)1);             // Version
        writer.Write(entries.Count);
        foreach (var entry in entries)
        {
            writer.Write(entry.Length + 1);
            writer.Write(entry.Header);
            writer.Write(entry.Data);
        }
    }
}

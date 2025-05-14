using Orion.Core.Server.Data.Directories;
using Prima.Core.Server.Data.Serialization;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.Server.Services;

public class PersistenceManager : IPersistenceManager
{
    private const byte Version = 1;
    private readonly ILogger<PersistenceManager> _logger;

    private readonly byte[] _magicNumber = "PRIMA"u8.ToArray();

    private readonly Dictionary<byte, IEntitySerializer> _entitySerializers = new();
    private readonly Dictionary<Type, IEntitySerializer> _entitySerializersAsType = new();


    public PersistenceManager(ILogger<PersistenceManager> logger)
    {
        _logger = logger;
    }


    public Task<List<TEntity>> DeserializeAsync<TEntity>(byte[] data) where TEntity : ISerializableEntity
    {
        throw new NotImplementedException();
    }

    public async Task<SerializationEntryData> SerializeAsync<TEntity>(TEntity entity) where TEntity : ISerializableEntity
    {
        ArgumentNullException.ThrowIfNull(entity);

        var serializer = _entitySerializersAsType[entity.GetType()];

        if (serializer is null)
        {
            throw new InvalidOperationException($"No serializer registered for entity type {entity.GetType()}");
        }

        var serializerData = serializer.Serialize(entity);

        return new SerializationEntryData(serializer.Header, serializerData, serializer.FileName);
    }

    public void RegisterEntitySerializer<TEntity>(IEntitySerializer<TEntity> serializer) where TEntity : ISerializableEntity
    {
        ArgumentNullException.ThrowIfNull(serializer);

        var type = typeof(TEntity);
        if (_entitySerializers.ContainsKey(serializer.Header))
        {
            _logger.LogWarning("Serializer for entity type 0x{Header:X2} is already registered. Overwriting.", serializer.Header);
        }

        _entitySerializersAsType[type] = serializer;
        _entitySerializers[serializer.Header] = serializer;
    }

    public async Task SaveToFileAsync<TEntity>(List<TEntity> entries, string fileName) where TEntity : ISerializableEntity
    {
        await using var stream = new FileStream(fileName, FileMode.CreateNew , FileAccess.Write, FileShare.None);

        await using var writer = new BinaryWriter(stream);

        writer.Write(_magicNumber); // Magic number
        writer.Write(Version);             // Version
        writer.Write((long)entries.Count);


        foreach (var entry in entries)
        {
            var data = await SerializeAsync(entry);

            writer.Write(data.Length);
            writer.Write((byte)data.Header);
            writer.Write(data.Data);
        }

    }

    public async Task<List<TEntity>> DeserializeAsync<TEntity>(string fileName) where TEntity : ISerializableEntity
    {
        var entries = new List<TEntity>();


        await using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new BinaryReader(stream);

        var magicNumber = reader.ReadBytes(5);
        if (!magicNumber.SequenceEqual(_magicNumber))
        {
            throw new InvalidOperationException("Invalid magic number");
        }

        var version = reader.ReadByte();
        if (version != Version)
        {
            throw new InvalidOperationException($"Unsupported version: {version}");
        }

        var entryCount = reader.ReadInt64();
        for (var i = 0; i < entryCount; i++)
        {
            var length = reader.ReadInt64();
            var header = reader.ReadByte();
            var data = reader.ReadBytes((int)length);

            if (_entitySerializers.TryGetValue(header, out var serializer))
            {
                entries.Add((TEntity)serializer.Deserialize(data));
            }
            else
            {
                _logger.LogWarning("No serializer registered for entity type {Type}", typeof(TEntity));
            }
        }

        return entries;
    }
}

using System.Reflection;
using Prima.Core.Server.Attributes;
using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.UOData.Serializers.Base;

public abstract class BaseEntitySerializer<TEntity> : IEntitySerializer<TEntity> where TEntity : class, ISerializableEntity
{
    public Type EntityType => typeof(TEntity);
    public byte Header { get; }

    protected BaseEntitySerializer()
    {
        var attribute = typeof(TEntity).GetCustomAttribute<SerializableHeaderAttribute>();

        if (attribute is null)
        {
            throw new InvalidOperationException(
                $"Entity {typeof(TEntity).Name} does not have a SerializableHeaderAttribute."
            );
        }

        Header = attribute.Header;
    }


    public byte[] Serialize(object entity, IPersistenceManager persistenceManager)
    {
        return Serialize(entity as TEntity, persistenceManager);
    }

    public byte[] Serialize(TEntity entity, IPersistenceManager persistenceManager)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        Serialize(writer, entity, persistenceManager);

        writer.Flush();
        return stream.ToArray();
    }

    public object Deserialize(byte[] data, IPersistenceManager persistenceManager)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);
        return Deserialize(reader, persistenceManager);
    }

    public abstract void Serialize(BinaryWriter writer, TEntity entity, IPersistenceManager persistenceManager);

    public abstract object Deserialize(BinaryReader reader, IPersistenceManager persistenceManager);
}

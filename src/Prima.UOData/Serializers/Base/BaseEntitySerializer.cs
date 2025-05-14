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


    public byte[] Serialize(object entity)
    {
        return Serialize(entity as TEntity);
    }

    public abstract object Deserialize(byte[] data);
    public abstract byte[] Serialize(TEntity entity);
}

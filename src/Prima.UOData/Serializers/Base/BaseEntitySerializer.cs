using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.UOData.Serializers.Base;

public abstract class BaseEntitySerializer<TEntity> : IEntitySerializer<TEntity> where TEntity : class, ISerializableEntity
{
    public Type EntityType => typeof(TEntity);

    public byte[] Serialize(object entity)
    {
        return Serialize(entity as TEntity);
    }

    object IEntitySerializer.Deserialize(byte[] data)
    {
        return Deserialize(data);
    }

    public abstract TEntity Deserialize(byte[] data);
    public abstract byte[] Serialize(TEntity entity);
}

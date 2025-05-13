namespace Prima.UOData.Interfaces.Persistence;

public interface IEntitySerializer
{
    Type EntityType { get; }

    byte[] Serialize(object entity);

    object Deserialize(byte[] data);
}

public interface IEntitySerializer<TEntity> : IEntitySerializer
{
    byte[] Serialize(TEntity entity);

    TEntity Deserialize(byte[] data);
}

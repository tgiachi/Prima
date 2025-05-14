namespace Prima.UOData.Interfaces.Persistence;

public interface IEntitySerializer
{
    Type EntityType { get; }

    byte Header { get; }

    byte[] Serialize(object entity);

    object Deserialize(byte[] data);
}

public interface IEntitySerializer<in TEntity> : IEntitySerializer
{
    byte[] Serialize(TEntity entity);


}

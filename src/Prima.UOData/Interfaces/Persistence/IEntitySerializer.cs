namespace Prima.UOData.Interfaces.Persistence;

public interface IEntitySerializer
{
    Type EntityType { get; }

    byte Header { get; }

    byte[] Serialize(object entity, IPersistenceManager persistenceManager);

    object Deserialize(byte[] data, IPersistenceManager persistenceManager);
}

public interface IEntitySerializer<in TEntity> : IEntitySerializer
{
    byte[] Serialize(TEntity entity, IPersistenceManager persistenceManager);
}

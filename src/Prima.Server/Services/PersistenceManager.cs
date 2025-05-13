using Prima.UOData.Interfaces.Persistence;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.Server.Services;

public class PersistenceManager : IPersistenceManager
{

    public Task<byte[]> SerializeAsync<TEntity>(params TEntity[] entity) where TEntity : ISerializableEntity
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> DeserializeAsync<TEntity>(byte[] data) where TEntity : ISerializableEntity
    {
        throw new NotImplementedException();
    }
}

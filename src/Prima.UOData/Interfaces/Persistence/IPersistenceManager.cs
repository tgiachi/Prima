using Orion.Core.Server.Interfaces.Services.Base;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.UOData.Interfaces.Persistence;

public interface IPersistenceManager : IOrionService
{
    Task<byte[]> SerializeAsync<TEntity>(params TEntity[] entity) where TEntity : ISerializableEntity;

    Task<List<TEntity>> DeserializeAsync<TEntity>(byte[] data) where TEntity : ISerializableEntity;
}

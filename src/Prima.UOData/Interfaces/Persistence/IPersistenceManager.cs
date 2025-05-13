using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Data.Serialization;
using Prima.UOData.Interfaces.Persistence.Entities;

namespace Prima.UOData.Interfaces.Persistence;

public interface IPersistenceManager : IOrionService
{
    Task<SerializationEntryData> SerializeAsync<TEntity>(TEntity entity) where TEntity : ISerializableEntity;

   // Task<List<TEntity>> DeserializeAsync<TEntity>(byte[] data) where TEntity : ISerializableEntity;

    void RegisterEntitySerializer<TEntity>(IEntitySerializer<TEntity> serializer) where TEntity : ISerializableEntity;
    Task SaveToFileAsync(List<SerializationEntryData> entries, string fileName);
}

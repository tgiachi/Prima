using Orion.Core.Server.Interfaces.Services.Base;
using Prima.UOData.Id;
using Prima.UOData.Interfaces.Entities;

namespace Prima.UOData.Interfaces.Services;

public interface IWorldManagerService : IOrionStartService, IOrionService
{
    void AddWorldEntity(IHaveSerial entity);

    TEntity GenerateWorldEntity<TEntity>() where TEntity : IHaveSerial;

    TEntity? GetEntityBySerial<TEntity>(Serial id) where TEntity : IHaveSerial;

    bool RemoveWorldEntity(IHaveSerial entity);

    Task SaveWorldAsync();
}

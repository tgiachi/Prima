using Orion.Core.Server.Interfaces.Services.Base;
using Prima.UOData.Interfaces.Entities;

namespace Prima.UOData.Interfaces.Services;

public interface IWorldManagerService : IOrionStartService, IOrionService
{
    void AddWorldEntity(IHaveSerial entity);

    TEntity GenerateWorldEntity<TEntity>() where TEntity : IHaveSerial;


    Task SaveWorldAsync();

}

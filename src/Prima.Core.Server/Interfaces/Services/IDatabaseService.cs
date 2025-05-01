using System.Linq.Expressions;
using Orion.Core.Server.Interfaces.Services.Base;
using Prima.Core.Server.Interfaces.Entities;

namespace Prima.Core.Server.Interfaces.Services;

public interface IDatabaseService : IOrionService, IOrionStartService
{
    Task<TEntity> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity;

    Task<List<TEntity>> InsertAsync<TEntity>(List<TEntity> entities) where TEntity : class, IPrimaDbEntity;

    Task<int> CountAsync<TEntity>() where TEntity : class, IPrimaDbEntity;

    Task<TEntity> FindByIdAsync<TEntity>(Guid id) where TEntity : class, IPrimaDbEntity;

    Task<IEnumerable<TEntity>> FindAllAsync<TEntity>() where TEntity : class, IPrimaDbEntity;

    Task<IEnumerable<TEntity>> QueryAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IPrimaDbEntity;

    Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IPrimaDbEntity;

    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity;

    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class, IPrimaDbEntity;

    Task DeleteAsync<TEntity>(Guid id) where TEntity : class, IPrimaDbEntity;

    Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IPrimaDbEntity;

    Task DeleteAllAsync<TEntity>() where TEntity : class, IPrimaDbEntity;

    Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IPrimaDbEntity;
}

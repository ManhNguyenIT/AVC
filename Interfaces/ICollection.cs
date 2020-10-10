using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVC.Interfaces
{
    public interface ICollection<TEntity> : IDisposable where TEntity : class
    {
        Task CreateAsync(TEntity obj);
        Task CreateManyAsync(IEnumerable<TEntity> obj);
        Task<TEntity> FindByIdAsync(string id);
        // Task<bool> IsExists(FieldDefinition<TEntity> field);
        Task<IEnumerable<TEntity>> GetsAsync(FilterDefinition<TEntity> filter = null);
        Task<TEntity> UpdateByIdAsync(string id, TEntity obj);
        Task<UpdateResult> UpdateManyAsync(FilterDefinition<TEntity> filter, UpdateDefinition<TEntity> update);
        Task<TEntity> DeleteByIdAsync(string id);
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<TEntity> filter = null);
    }
}

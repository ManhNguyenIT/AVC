using AVC.Collections;
using AVC.DatabaseModels;
using AVC.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVC.Services
{
    public class TotalService : ITotalService
    {
        private readonly ITotalCollection _totalCollection;

        public TotalService(ITotalCollection totalCollection)
        {
            _totalCollection = totalCollection;
        }
        public Task CreateAsync(Total obj)
        {
            return _totalCollection.CreateAsync(obj);
        }

        public Task CreateManyAsync(IEnumerable<Total> obj)
        {
            return _totalCollection.CreateManyAsync(obj);
        }

        public Task<Total> DeleteByIdAsync(string id)
        {
            return _totalCollection.DeleteByIdAsync(id);
        }

        public Task<Total> UpdateByIdAsync(string id, Total obj)
        {
            return _totalCollection.UpdateByIdAsync(id, obj);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<Total> filter)
        {
            return await _totalCollection.DeleteManyAsync(filter);
        }

        public async Task<Total> FindByIdAsync(string id)
        {
            return await _totalCollection.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Total>> GetsAsync(FilterDefinition<Total> filter)
        {
            return await _totalCollection.GetsAsync(filter);
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<Total> filter, UpdateDefinition<Total> update, UpdateOptions options = null)
        {
            return await _totalCollection.UpdateManyAsync(filter, update, options);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~TotalService()
        {
            Dispose();
        }
    }
}
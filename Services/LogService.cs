using AVC.Collections;
using AVC.DatabaseModels;
using AVC.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVC.Services
{
    public class LogService : ILogService
    {
        private readonly ILogCollection _LogCollection;

        public LogService(ILogCollection LogCollection)
        {
            _LogCollection = LogCollection;
        }
        public Task CreateAsync(Log obj)
        {
            return _LogCollection.CreateAsync(obj);
        }

        public Task CreateManyAsync(IEnumerable<Log> obj)
        {
            return _LogCollection.CreateManyAsync(obj);
        }

        public Task<Log> DeleteByIdAsync(string id)
        {
            return _LogCollection.DeleteByIdAsync(id);
        }

        public Task<Log> UpdateByIdAsync(string id, Log obj)
        {
            return _LogCollection.UpdateByIdAsync(id, obj);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<Log> filter)
        {
            return await _LogCollection.DeleteManyAsync(filter);
        }

        public async Task<Log> FindByIdAsync(string id)
        {
            return await _LogCollection.FindByIdAsync(id);
        }

        public async Task<Log> FindByIpAsync(string ip)
        {
            return await _LogCollection.FindByIpAsync(ip);
        }

        public async Task<IEnumerable<Log>> GetsAsync(FilterDefinition<Log> filter)
        {
            return await _LogCollection.GetsAsync(filter);
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<Log> filter, UpdateDefinition<Log> update)
        {
            return await _LogCollection.UpdateManyAsync(filter, update);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~LogService()
        {
            Dispose();
        }
    }
}
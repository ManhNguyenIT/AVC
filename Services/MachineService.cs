using AVC.Collections;
using AVC.DatabaseModels;
using AVC.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AVC.Services
{
    public class MachineService : IMachineService
    {
        private readonly IMachineCollection _MachineCollection;

        public MachineService(IMachineCollection MachineCollection)
        {
            _MachineCollection = MachineCollection;
        }
        public Task CreateAsync(Machine obj)
        {
            return _MachineCollection.CreateAsync(obj);
        }

        public Task CreateManyAsync(IEnumerable<Machine> obj)
        {
            return _MachineCollection.CreateManyAsync(obj);
        }

        public Task<Machine> DeleteByIdAsync(string id)
        {
            return _MachineCollection.DeleteByIdAsync(id);
        }

        public Task<Machine> UpdateByIdAsync(string id, Machine obj)
        {
            return _MachineCollection.UpdateByIdAsync(id, obj);
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<Machine> filter)
        {
            return await _MachineCollection.DeleteManyAsync(filter);
        }

        public async Task<Machine> FindByIdAsync(string id)
        {
            return await _MachineCollection.FindByIdAsync(id);
        }

        public async Task<Machine> FindByIpAsync(string ip)
        {
            return await _MachineCollection.FindByIpAsync(ip);
        }

        public async Task<IEnumerable<Machine>> GetsAsync(FilterDefinition<Machine> filter)
        {
            return await _MachineCollection.GetsAsync(filter);
        }

        public async Task<UpdateResult> UpdateManyAsync(FilterDefinition<Machine> filter, UpdateDefinition<Machine> update)
        {
            return await _MachineCollection.UpdateManyAsync(filter, update);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~MachineService()
        {
            Dispose();
        }
    }
}
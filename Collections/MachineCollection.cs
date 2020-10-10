using System.Threading.Tasks;
using AVC.DatabaseModels;
using AVC.Models;
using MongoDB.Driver;

namespace AVC.Collections
{
    public interface IMachineCollection : Interfaces.ICollection<Machine>
    {
        Task<Machine> FindByIpAsync(string ip);
    }
    public class MachineCollection : BaseCollection<Machine>, IMachineCollection
    {
        public MachineCollection(IMongoContext context) : base(context)
        {
        }

        public async Task<Machine> FindByIpAsync(string ip)
        {
            return await _collection.Find(Builders<Machine>.Filter.Eq("ip", ip)).FirstOrDefaultAsync();
        }
    }
}
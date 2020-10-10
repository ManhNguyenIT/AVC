using System.Threading.Tasks;
using AVC.DatabaseModels;
using AVC.Models;
using MongoDB.Driver;

namespace AVC.Collections
{
    public interface ILogCollection : Interfaces.ICollection<Log>
    {
        Task<Log> FindByIpAsync(string ip);
    }
    public class LogCollection : BaseCollection<Log>, ILogCollection
    {
        public LogCollection(IMongoContext context) : base(context)
        {
        }

        public async Task<Log> FindByIpAsync(string ip)
        {
            return await _collection.Find(Builders<Log>.Filter.Eq("ip", ip)).FirstOrDefaultAsync();
        }
    }
}
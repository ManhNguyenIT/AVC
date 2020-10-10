using System.Threading.Tasks;
using AVC.DatabaseModels;

namespace AVC.Interfaces
{
    public interface ILogService : ICollection<Log>
    {
        Task<Log> FindByIpAsync(string ip);
    }
}

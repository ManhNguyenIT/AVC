using System.Threading.Tasks;
using AVC.DatabaseModels;

namespace AVC.Interfaces
{
    public interface IMachineService : ICollection<Machine>
    {
        Task<Machine> FindByIpAsync(string ip);
    }
}

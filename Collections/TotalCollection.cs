using System.Threading.Tasks;
using AVC.DatabaseModels;
using AVC.Models;
using MongoDB.Driver;

namespace AVC.Collections
{
    public interface ITotalCollection : Interfaces.ICollection<Total>
    {
    }
    public class TotalCollection : BaseCollection<Total>, ITotalCollection
    {
        public TotalCollection(IMongoContext context) : base(context)
        {
        }
    }
}
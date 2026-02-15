using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Points
{
    // ICityManager
    public interface IPointManager : IDomainService
    {

        Task<Point> GetEntityByIdAsync(int id);

    }
}

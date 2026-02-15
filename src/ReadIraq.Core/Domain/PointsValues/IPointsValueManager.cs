using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PointsValues
{
    public interface IPointsValueManager : IDomainService
    {
        Task InsertPointsValue(PointsValue pointsValue);
    }
}

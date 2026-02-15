using Abp.Domain.Repositories;
using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PointsValues
{
    public class PointsValueManager : DomainService, IPointsValueManager
    {
        private readonly IRepository<PointsValue> _pointsValueRepository;

        public PointsValueManager(IRepository<PointsValue> repository)
        {
            _pointsValueRepository = repository;
        }

        public async Task InsertPointsValue(PointsValue pointsValue)
        {
            await _pointsValueRepository.InsertAndGetIdAsync(pointsValue);
        }
    }
}

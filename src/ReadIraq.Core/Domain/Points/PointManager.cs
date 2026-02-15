using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Points;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Cities
{
    //city manager
    public class PointManager : DomainService, IPointManager
    {

        private readonly IRepository<Point> _pointRepository;
        public PointManager(IRepository<Point> pointRepository)
        {
            _pointRepository = pointRepository;
        }

        public async Task<Point> GetEntityByIdAsync(int id)
        {
            var entity = await _pointRepository.GetAll().Include(x => x.Translations).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(Point), id);
            return entity;
        }
    }
}

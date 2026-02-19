using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Advertisiments
{
    public class AdvertisimentManager : DomainService, IAdvertisimentManager
    {
        private readonly IRepository<Advertisiment> _advertisimentrepository;

        public AdvertisimentManager(IRepository<Advertisiment> advertisimentRepository)
        {
            _advertisimentrepository = advertisimentRepository;
        }

        public async Task<Advertisiment> CheckAdvertisiment(int Id)
        {
            return await _advertisimentrepository.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Advertisiment> GetEntityAsync(int Id)
        {
            return await _advertisimentrepository.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<Advertisiment> InsertAsync(Advertisiment advertisiment)
        {
            return await _advertisimentrepository.InsertAsync(advertisiment);
        }
    }
}

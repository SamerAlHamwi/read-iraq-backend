using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Advertisiments
{
    public interface IAdvertisimentManager : IDomainService
    {
        Task<Advertisiment> CheckAdvertisiment(int Id);
        Task<Advertisiment> GetEntityAsync(int Id);
        Task<Advertisiment> InsertAsync(Advertisiment advertisiment);
        Task AddPositionToAdvertisimentAsync(AdvertisimentPosition advertisimentPosition);
        Task RemovePositionFromAdvertisiment(AdvertisimentPosition position);
        Task<List<AdvertisimentPosition>> GetAdvertisimentPositionsAsync(int advertisimentId);


    }
}

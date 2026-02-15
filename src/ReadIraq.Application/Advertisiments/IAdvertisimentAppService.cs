using ReadIraq.Advertisiments.Dto;
using ReadIraq.CrudAppServiceBase;
using System.Threading.Tasks;


namespace ReadIraq.Advertisiments
{
    public interface IAdvertisimentAppService : IReadIraqAsyncCrudAppService<AdvertisimentDetailsDto, int, LiteAdvertisimentDto, PagedAdvertisimentResultRequestDto,
        CreateAdvertisimentDto, UpdateAdvertisimentDto>
    {
        Task AddAdvertisimentPositionToAdvertisiment(AddAdvertisimentPositionDto input);
    }
}

using ReadIraq.Advertisiments.Dto;
using ReadIraq.CrudAppServiceBase;

namespace ReadIraq.Advertisiments
{
    public interface IAdvertisimentAppService : IReadIraqAsyncCrudAppService<AdvertisimentDetailsDto, int, LiteAdvertisimentDto, PagedAdvertisimentResultRequestDto,
        CreateAdvertisimentDto, UpdateAdvertisimentDto>
    {
    }
}

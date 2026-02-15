using ReadIraq.Cities.Dto;
using ReadIraq.CrudAppServiceBase;

namespace ReadIraq.Cities
{
    public interface ICityAppService : IReadIraqAsyncCrudAppService<CityDetailsDto, int, LiteCityDto, PagedCityResultRequestDto,
        CreateCityDto, UpdateCityDto>
    {

    }
}

using ReadIraq.Countries.Dto;
using ReadIraq.CrudAppServiceBase;

namespace ReadIraq.Countries
{
    public interface ICountryAppService : IReadIraqAsyncCrudAppService<CountryDetailsDto, int, CountryDto, PagedCountryResultRequestDto,
        CreateCountryDto, UpdateCountryDto>
    {


    }
}

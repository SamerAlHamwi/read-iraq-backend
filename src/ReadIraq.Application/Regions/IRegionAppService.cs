
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Regions.Dto;

namespace ClinicSystem.Regions
{
    public interface IRegionAppService : IReadIraqAsyncCrudAppService<RegionDetailsDto, int, LiteRegionDto
        , PagedRegionResultRequestDto,
        CreateRegionDto, UpdateRegionDto>
    {

    }
}

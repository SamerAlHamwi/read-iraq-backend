using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.ApkBuilds.Dtos;

namespace ReadIraq.ApkBuildAppService
{
    public interface IApkBuildAppService : IReadIraqAsyncCrudAppService<ApkBuildDetailsDto, int, LiteApkBuildDto, PagedApkBuildResultRequestDto,
        CreateApkBuildDto, UpdateApkBuildDto>
    {
    }
}

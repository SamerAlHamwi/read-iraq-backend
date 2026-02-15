using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ApkBuilds.Dtos
{
    public class PagedApkBuildResultRequestDto : PagedResultRequestDto
    {
        public AppType? AppType { get; set; }
        public SystemType? SystemType { get; set; }
        public UpdateOptions? UpdateOptions { get; set; }

    }
}

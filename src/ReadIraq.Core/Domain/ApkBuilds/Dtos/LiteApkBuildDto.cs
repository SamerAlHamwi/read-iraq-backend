using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ApkBuilds.Dtos
{
    public class LiteApkBuildDto : EntityDto
    {
        public SystemType SystemType { get; set; }
        public int VersionCode { get; set; }
        public string VersionNumber { get; set; }
        public string Description { get; set; }
        public UpdateOptions UpdateOptions { get; set; }
    }
}

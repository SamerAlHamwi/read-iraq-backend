using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.ApkBuilds.Dtos
{
    public class UpdateApkBuildDto : CreateApkBuildDto, IEntityDto
    {
        public int Id { get; set; }
    }
}

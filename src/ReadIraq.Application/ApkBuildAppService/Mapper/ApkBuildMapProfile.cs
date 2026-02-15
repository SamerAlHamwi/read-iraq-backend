using AutoMapper;
using ReadIraq.Domain.ApkBuilds;
using ReadIraq.Domain.ApkBuilds.Dtos;

namespace ReadIraq.ApkBuildAppService.Mapper
{
    public class ApkBuildMapProfile : Profile
    {
        public ApkBuildMapProfile()
        {
            CreateMap<CreateApkBuildDto, ApkBuild>();
            CreateMap<ApkBuild, LiteApkBuildDto>();
            CreateMap<ApkBuild, ApkBuildDetailsDto>();
            CreateMap<UpdateApkBuildDto, ApkBuild>();
        }
    }
}

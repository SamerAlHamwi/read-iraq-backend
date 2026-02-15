using AutoMapper;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.Regions.Dto;

namespace ReadIraq.Regions.Mapper
{
    public class RegionMapProfile : Profile
    {
        public RegionMapProfile()
        {
            CreateMap<CreateRegionDto, Region>();
            CreateMap<CreateRegionDto, RegionDto>();
            CreateMap<RegionDto, Region>();
            CreateMap<UpdateRegionDto, Region>();
            CreateMap<LiteRegionDto, Region>();
        }
    }
}

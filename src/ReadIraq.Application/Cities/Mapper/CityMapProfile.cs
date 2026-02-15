using AutoMapper;
using ReadIraq.Cities.Dto;
using ReadIraq.Domain.Cities;

namespace ReadIraq.Cities.Mapper
{
    public class CityMapProfile : Profile
    {
        public CityMapProfile()
        {
            CreateMap<CreateCityDto, City>();
            CreateMap<CreateCityDto, CityDto>();
            CreateMap<CityDto, City>();
            CreateMap<UpdateCityDto, City>();
            CreateMap<LiteCity, City>();

        }
    }
}

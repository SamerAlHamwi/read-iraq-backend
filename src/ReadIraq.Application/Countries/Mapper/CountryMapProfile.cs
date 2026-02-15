using AutoMapper;
using ReadIraq.Countries.Dto;

namespace ReadIraq.Countries.Mapper
{
    public class CountryMapProfile : Profile
    {
        public CountryMapProfile()
        {
            CreateMap<CreateCountryDto, Country>();
            CreateMap<CreateCountryDto, CountryDto>();
            CreateMap<CountryDto, Country>();
            CreateMap<Country, UpdateCountryDto>();
            CreateMap<UpdateCountryDto, Country>();
        }
    }
}

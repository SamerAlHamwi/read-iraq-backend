using AutoMapper;
using ReadIraq.Advertisiments.Dto;
using ReadIraq.Domain.Advertisiments;
using ReadIraq.Domain.Attachments;

namespace ReadIraq.Advertisiments.Mapper
{
    public class AdvertisimentMapProfile : Profile
    {
        public AdvertisimentMapProfile()
        {
            CreateMap<Advertisiment, UpdateAdvertisimentDto>();
            CreateMap<CreateAdvertisimentDto, Advertisiment>();
            CreateMap<Advertisiment, CreateAdvertisimentDto>();
            CreateMap<Advertisiment, LiteAdvertisimentDto>();
            CreateMap<Advertisiment, AdvertisimentDetailsDto>();
            CreateMap<Attachment, string>().ConvertUsing(source => source.StorageKey ?? string.Empty);
        }
    }
}

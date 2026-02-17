using AutoMapper;
using ReadIraq.Advertisiments.Dto;
using ReadIraq.Domain.Advertisiments;
using ReadIraq.Domain.Attachments;

namespace ReadIraq.Advertisiments.Mapper
{
    /// <summary>
    /// PostsMapProfile
    /// </summary>
    public class AdvertisimentMapProfile : Profile
    {
        /// <summary>
        ///  Posts Map Profile 
        /// </summary>
        public AdvertisimentMapProfile()
        {
            CreateMap<Advertisiment, UpdateAdvertisimentDto>();
            CreateMap<CreateAdvertisimentDto, Advertisiment>();
            CreateMap<Advertisiment, CreateAdvertisimentDto>();
            CreateMap<Advertisiment, LiteAdvertisimentDto>();
            CreateMap<Advertisiment, AdvertisimentDetailsDto>();
            CreateMap<Attachment, string>().ConvertUsing(source => source.StorageKey ?? string.Empty);
            CreateMap<CreateAdvertisimentPositionDto, AdvertisimentPosition>();
            CreateMap<AdvertisimentPosition, AdvertisimentPositionDto>();
            CreateMap<AddAdvertisimentPositionDto, AdvertisimentPosition>();
        }



    }
}

using AutoMapper;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Translations.Dto;

namespace ReadIraq.Translations.Mapper
{
    public class TranslationMapProfile : Profile
    {
        public TranslationMapProfile()
        {
            CreateMap<CreateTranslationDto, Translation>();
            CreateMap<UpdateTranslationDto, Translation>();
            CreateMap<Translation, TranslationDto>();
        }
    }
}

using AutoMapper;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.FrequentlyQuestions.Dto;

namespace ReadIraq.FrequentlyQuestionService.Mapper
{
    public class FrequentlyQuestionMapProfile : Profile
    {
        public FrequentlyQuestionMapProfile()
        {
            CreateMap<CreateFrequentlyQuestionDto, FrequentlyQuestion>();
            //CreateMap<FrequentlyQuestion, FrequentlyQuestionDetailsDto>();
        }
    }
}

using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.FrequentlyQuestions.Dto
{
    public class UpdateFrequentlyQuestionDto : CreateFrequentlyQuestionDto, IEntityDto
    {
        public int Id { get; set; }
    }
}

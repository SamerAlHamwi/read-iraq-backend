using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.FrequentlyQuestions.Dto
{
    public class LiteFrequentlyQuestionDto : EntityDto<int>
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public List<FrequentlyQuestionTranslationDto> Translations { get; set; }
    }
}

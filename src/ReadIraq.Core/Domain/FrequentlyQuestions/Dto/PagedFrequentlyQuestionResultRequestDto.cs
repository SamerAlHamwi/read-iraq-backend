using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.FrequentlyQuestions.Dto
{
    public class PagedFrequentlyQuestionResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public AppType? App { get; set; }

    }
}

using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Translations.Dto
{
    public class PagedTranslationResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

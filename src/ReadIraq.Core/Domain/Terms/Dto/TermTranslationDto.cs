using Abp.AutoMapper;
using ReadIraq.Domain.Terms;

namespace ReadIraq.TermService.Dto
{
    [AutoMap(typeof(TermTranslation))]

    public class TermTranslationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }

    }
}

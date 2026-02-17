using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Subjects.Dto
{
    public class PagedSubjectResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public SubjectLevel? Level { get; set; }
    }
}

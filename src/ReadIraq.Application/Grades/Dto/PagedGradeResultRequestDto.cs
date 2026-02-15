using Abp.Application.Services.Dto;

namespace ReadIraq.Grades.Dto
{
    public class PagedGradeResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

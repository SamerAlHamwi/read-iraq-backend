using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Grades.Dto
{
    public class PagedGradeGroupResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

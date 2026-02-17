using Abp.Application.Services.Dto;

namespace ReadIraq.Teachers.Dto
{
    public class PagedTeacherFeatureResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

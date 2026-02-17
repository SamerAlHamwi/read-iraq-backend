using Abp.Application.Services.Dto;

namespace ReadIraq.Teachers.Dto
{
    public class PagedTeacherProfileResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

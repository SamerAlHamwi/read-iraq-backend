using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Teachers.Dto
{
    public class PagedTeacherReviewResultRequestDto : PagedAndSortedResultRequestDto
    {
        public Guid? TeacherProfileId { get; set; }
        public string Keyword { get; set; }
    }
}

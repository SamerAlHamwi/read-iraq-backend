using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Teachers.Dto
{
    public class PagedTeacherProfileResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public Guid? SubjectId { get; set; }
        public int? GradeId { get; set; }
        public bool? IsActive { get; set; }
    }
}

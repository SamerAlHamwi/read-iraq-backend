using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Enrollments.Dto
{
    public class PagedEnrollmentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? UserId { get; set; }
        public Guid? SubjectId { get; set; }
    }
}

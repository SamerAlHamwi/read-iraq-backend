using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.UserPreferredSubjects.Dto
{
    public class PagedUserPreferredSubjectResultRequestDto : PagedAndSortedResultRequestDto
    {
        public long? UserId { get; set; }
        public Guid? SubjectId { get; set; }
    }
}

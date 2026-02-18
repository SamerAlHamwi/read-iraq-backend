using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.LessonSessions.Dto
{
    public class PagedLessonSessionResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public Guid? TeacherProfileId { get; set; }
        public Guid? SubjectId { get; set; }
        public bool? IsFree { get; set; }
        public bool? IsActive { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;

namespace ReadIraq.Domain.LessonSessions.Dto
{
    [AutoMapFrom(typeof(LessonReport))]
    public class LessonReportDto : FullAuditedEntityDto<Guid>
    {
        public Guid LessonSessionId { get; set; }
        public string LessonSessionTitle { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public bool IsProcessed { get; set; }
    }

    [AutoMapTo(typeof(LessonReport))]
    public class CreateLessonReportDto
    {
        public Guid LessonSessionId { get; set; }
        public string Message { get; set; }
    }

    public class PagedLessonReportResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsProcessed { get; set; }
    }
}

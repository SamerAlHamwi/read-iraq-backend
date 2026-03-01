using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherReport))]
    public class TeacherReportDto : FullAuditedEntityDto<Guid>
    {
        public Guid TeacherProfileId { get; set; }
        public string TeacherName { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public bool IsProcessed { get; set; }
    }

    [AutoMapTo(typeof(TeacherReport))]
    public class CreateTeacherReportDto
    {
        public Guid TeacherProfileId { get; set; }
        public string Message { get; set; }
    }

    public class PagedTeacherReportResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsProcessed { get; set; }
    }
}

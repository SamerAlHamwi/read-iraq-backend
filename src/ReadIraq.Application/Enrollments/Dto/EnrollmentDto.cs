using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Enrollments;
using System;

namespace ReadIraq.Enrollments.Dto
{
    [AutoMapFrom(typeof(Enrollment))]
    public class EnrollmentDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid? TeacherId { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

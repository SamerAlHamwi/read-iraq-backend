using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Enrollments;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Enrollments.Dto
{
    [AutoMapTo(typeof(Enrollment))]
    public class UpdateEnrollmentDto : EntityDto<Guid>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid SubjectId { get; set; }

        public Guid? TeacherId { get; set; }

        public decimal ProgressPercent { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subjects;
using System;

namespace ReadIraq.UserPreferredSubjects.Dto
{
    [AutoMapTo(typeof(UserPreferredSubject))]
    public class UpdateUserPreferredSubjectDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid SubjectId { get; set; }
        public Guid? TeacherId { get; set; }
        public decimal ProgressPercent { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}

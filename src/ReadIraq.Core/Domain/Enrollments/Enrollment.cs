using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Enrollments
{
    [Table("Enrollments")]
    public class Enrollment : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public Guid? TeacherId { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual TeacherProfile Teacher { get; set; }

        public int? GradeId { get; set; }

        [ForeignKey(nameof(GradeId))]
        public virtual Grade Grade { get; set; }

        public decimal ProgressPercent { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }
    }
}

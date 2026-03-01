using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Grades;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherSubjects")]
    public class TeacherSubject : FullAuditedEntity<Guid>
    {
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public int GradeId { get; set; }

        [ForeignKey(nameof(GradeId))]
        public virtual Grade Grade { get; set; }
    }
}

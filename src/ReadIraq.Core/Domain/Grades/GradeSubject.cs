using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Subjects;

namespace ReadIraq.Domain.Grades
{
    [Table("GradeSubjects")]
    public class GradeSubject : FullAuditedEntity
    {
        public int GradeId { get; set; }

        [ForeignKey(nameof(GradeId))]
        public virtual Grade Grade { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }
    }
}

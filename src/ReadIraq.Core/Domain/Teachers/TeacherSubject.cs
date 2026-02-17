using Abp.Domain.Entities;
using ReadIraq.Domain.Subjects;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherSubjects")]
    public class TeacherSubject : Entity
    {
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }
    }
}

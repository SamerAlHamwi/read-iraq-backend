using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Subjects;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("UserPreferredTeachers")]
    public class UserPreferredTeacher : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Follows
{
    [Table("UserFollowTeachers")]
    public class UserFollowTeacher : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }
    }
}

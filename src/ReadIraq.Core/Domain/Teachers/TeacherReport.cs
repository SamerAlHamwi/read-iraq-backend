using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherReports")]
    public class TeacherReport : FullAuditedEntity<Guid>
    {
        public Guid TeacherProfileId { get; set; }
        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public string Message { get; set; }

        public bool IsProcessed { get; set; }
    }
}

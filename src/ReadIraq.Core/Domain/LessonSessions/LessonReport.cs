using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.LessonSessions
{
    [Table("LessonReports")]
    public class LessonReport : FullAuditedEntity<Guid>
    {
        public Guid LessonSessionId { get; set; }
        [ForeignKey(nameof(LessonSessionId))]
        public virtual LessonSession LessonSession { get; set; }

        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public string Message { get; set; }

        public bool IsProcessed { get; set; }
    }
}

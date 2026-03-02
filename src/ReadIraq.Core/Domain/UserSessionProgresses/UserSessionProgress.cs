using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.LessonSessions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.UserSessionProgresses
{
    [Table("UserSessionProgresses")]
    public class UserSessionProgress : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public virtual LessonSession Session { get; set; }

        public int WatchedSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public bool CanTakeQuiz { get; set; }

        public DateTime LastWatchedAt { get; set; }
    }
}

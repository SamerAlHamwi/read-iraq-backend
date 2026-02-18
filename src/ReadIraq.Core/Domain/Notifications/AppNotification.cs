using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Notifications
{
    [Table("AppNotifications")]
    public class AppNotification : FullAuditedEntity<Guid>
    {
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        /// <summary>
        /// Metadata/Deep link info, stored as JSON string.
        /// </summary>
        public string Data { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ScheduledAt { get; set; }
    }
}

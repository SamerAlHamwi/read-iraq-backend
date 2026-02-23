using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Notifications
{
    [Table("Notifications")]
    public class AppNotification : FullAuditedEntity<Guid>
    {
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public NotificationType Type { get; set; }

        /// <summary>
        /// {"en":"...", "ar":"..."} (store as JSON)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// {"en":"...", "ar":"..."} (store as JSON)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Metadata/Deep link info, stored as JSON string.
        /// </summary>
        public string Data { get; set; }

        public NotificationPriority Priority { get; set; }
        public NotificationChannel Channel { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ScheduledAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }

        public string FCMMessageId { get; set; }
        public NotificationDeliveryStatus DeliveryStatus { get; set; }

        /// <summary>
        /// Any other metadata (attempts, errors)
        /// </summary>
        public string Meta { get; set; }
    }
}

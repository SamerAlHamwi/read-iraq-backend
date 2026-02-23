using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Notifications;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Notifications.Dto
{
    [AutoMapFrom(typeof(AppNotification))]
    public class AppNotificationDto : EntityDto<Guid>
    {
        public NotificationType Type { get; set; }
        public string Title { get; set; } // Localized
        public string Body { get; set; }  // Localized
        public string Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [AutoMapTo(typeof(AppNotification))]
    public class CreateAppNotificationDto
    {
        public long? UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } // JSON
        public string Body { get; set; }  // JSON
        public string Data { get; set; }
        public NotificationPriority Priority { get; set; }
        public NotificationChannel Channel { get; set; }
        public DateTime? ScheduledAtUtc { get; set; }
    }

    public class SendNotificationInput
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public NotificationType Type { get; set; }
        public NotificationFilter Filter { get; set; }
    }

    public class NotificationFilter
    {
        public bool All { get; set; }
        public long[] UserIds { get; set; }
    }

    public class ScheduleNotificationInput
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime ScheduledTime { get; set; }
        public long[] UserIds { get; set; }
    }
}

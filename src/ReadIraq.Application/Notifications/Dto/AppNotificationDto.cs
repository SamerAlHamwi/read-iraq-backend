using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Notifications;
using System;

namespace ReadIraq.Notifications.Dto
{
    [AutoMapFrom(typeof(AppNotification))]
    public class AppNotificationDto : EntityDto<Guid>
    {
        public long? UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime CreationTime { get; set; }
    }

    [AutoMapTo(typeof(AppNotification))]
    public class CreateAppNotificationDto
    {
        public long? UserId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}

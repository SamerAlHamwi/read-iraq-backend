using Abp.Application.Services.Dto;

namespace ReadIraq.PushNotifications.Dto
{
    public class PushNotificationDto : EntityDto<int>
    {
        public string Message { get; set; }
    }
}

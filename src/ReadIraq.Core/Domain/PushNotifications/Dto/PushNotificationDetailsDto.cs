using Abp.Application.Services.Dto;

namespace ReadIraq.PushNotifications.Dto
{
    public class PushNotificationDetailsDto : EntityDto
    {
        public string Message { get; set; }
    }
}

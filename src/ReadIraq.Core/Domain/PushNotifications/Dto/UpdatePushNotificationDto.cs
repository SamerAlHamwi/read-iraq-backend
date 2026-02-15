using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.PushNotifications.Dto
{
    public class UpdatePushNotificationDto : CreatePushNotificationDto, IEntityDto
    {
        [Required]
        public int Id { get; set; }
    }
}

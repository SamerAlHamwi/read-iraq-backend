using Abp.AutoMapper;
using ReadIraq.Domain.PushNotifications;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.PushNotifications.Dto
{
    [AutoMap(typeof(PushNotificationTranslation))]
    public class PushNotificationTranslationDto
    {
        [Required]
        public string Message { get; set; }
        [Required]
        public string Language { get; set; }
    }
}

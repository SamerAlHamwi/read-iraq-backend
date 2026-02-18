using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PushNotifications.Dto
{
    public class CreatePushNotificationDto
    {
        public List<PushNotificationTranslationDto> Translations { get; set; }

    }
}

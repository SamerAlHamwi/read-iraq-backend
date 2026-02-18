using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PushNotifications.Dto
{
    public class LitePushNotificationDto : EntityDto<int>
    {
        public List<PushNotificationTranslationDto> Translations { get; set; }
        [JsonIgnore]
        public string DestinationText { get; set; }
        public string ArTitle { get; set; }
        public string EnTitle { get; set; }
    }
}


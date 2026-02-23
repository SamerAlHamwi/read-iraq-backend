using Abp.Notifications;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.NotificationService
{
    [Serializable]
    public class TypedMessageNotificationData : NotificationData
    {
        public NotificationType NotificationType { get; set; }

        // These will now store the localized values retrieved during notification creation
        public string ArTitle { get; set; }
        public string EnTitle { get; set; }
        public string ArMessage { get; set; }
        public string EnMessage { get; set; }

        public string AdditionalValue { get; set; }

        public TypedMessageNotificationData(
            NotificationType notificationType,
            string arTitle,
            string enTitle,
            string arMessage,
            string enMessage,
            string additionalValue)
        {
            NotificationType = notificationType;
            ArTitle = arTitle;
            EnTitle = enTitle;
            ArMessage = arMessage;
            EnMessage = enMessage;
            AdditionalValue = additionalValue;

            Properties.Add(nameof(NotificationType), NotificationType);
            Properties.Add(nameof(AdditionalValue), AdditionalValue);
            Properties.Add(nameof(ArTitle), ArTitle);
            Properties.Add(nameof(EnTitle), EnTitle);
            Properties.Add(nameof(ArMessage), ArMessage);
            Properties.Add(nameof(EnMessage), EnMessage);
        }
    }
}

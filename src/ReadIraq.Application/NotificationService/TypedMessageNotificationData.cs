using Abp.Localization.Sources;
using Abp.Notifications;
using System;
using System.Globalization;

namespace ReadIraq.NotificationService
{
    [Serializable]
    public class TypedMessageNotificationData : NotificationData
    {
        public NotificationType NotificationType { get; set; }
        public string ArMessage { get; set; }
        public string EnMessage { get; set; }

        public string AdditionalValue { get; set; }

        public TypedMessageNotificationData(NotificationType notificationType, string arMessage, string enMessage, string AdditionalValue)
        {
            NotificationType = notificationType;
            ArMessage = arMessage;
            EnMessage = enMessage;
            AdditionalValue = AdditionalValue;

            Properties.Add(nameof(NotificationType), NotificationType);
            Properties.Add(nameof(AdditionalValue), AdditionalValue);
            Properties.Add(nameof(arMessage), arMessage);
            Properties.Add(nameof(enMessage), enMessage);
        }

        public static TypedMessageNotificationData CreateCustom(NotificationType notificationType, ILocalizationSource localizationSource,
           string AdditionalValue, string arMessage, string enMessage)
        {
            return new TypedMessageNotificationData(notificationType, arMessage, enMessage, AdditionalValue);
        }

        public static TypedMessageNotificationData Create(NotificationType notificationType, ILocalizationSource localizationSource,
            string AdditionalValue, params object[] localizationParams)
        {
            var arMessage = localizationSource.GetString(notificationType + "Text",
                    CultureInfo.GetCultureInfo("ar"));
            var enMessage = localizationSource.GetString(notificationType + "Text",
                    CultureInfo.GetCultureInfo("en"));

            return new TypedMessageNotificationData(notificationType, arMessage, enMessage, AdditionalValue);
        }
    }

    public enum NotificationType : byte
    {
        PushNotification = 1,
        NewRequest = 2,
        NewOffer = 3,
        AskForHelp = 4,
        ApproveForCompany = 5,
        PossibleClient = 6,
        UpdateCompany = 7,

    }
}
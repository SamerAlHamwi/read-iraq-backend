using Abp.Localization;
using Abp.Localization.Sources;
using ReadIraq.NotificationService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace ReadIraq.NotificationSender
{
    public class NotificationSender : INotificationSender
    {
        private readonly ILocalizationSource _localizationSource;
        private readonly INotificationService _InotificationService;

        public NotificationSender(ILocalizationManager localizationManager, INotificationService inotificationService)
        {
            _localizationSource = localizationManager.GetSource(ReadIraqConsts.LocalizationSourceName);
            _InotificationService = inotificationService;
        }

    }
}

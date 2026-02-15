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

        public async Task SendNotificationForAdminToNoticHimAboutRequestToUpdateCompany(List<long> userIds, int parentCompanyId)
        {
            var enMessage = _localizationSource.GetString("ThereIsCompanyNeedToUpdate", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("ThereIsCompanyNeedToUpdate", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.UpdateCompany, arMessage, enMessage, "");
            data.Properties.Add("ParentCompanyId", parentCompanyId);
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true);
        }

        public async Task SendNotificationForCompanyForApproved(List<long> userIds)
        {
            var enMessage = _localizationSource.GetString("YourCompanyHasApproved", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("YourCompanyHasApproved", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.ApproveForCompany, arMessage, enMessage, "");
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationForCompanyForApproveUpdate(List<long> userIds)
        {
            var enMessage = _localizationSource.GetString("YourUpdatedHasApproved", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("YourUpdatedHasApproved", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.UpdateCompany, arMessage, enMessage, "");
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationForCostumerServiceForAskHelp(List<long> userIds, long userId)
        {
            var enMessage = _localizationSource.GetString("ThereIsNewAskForHelp", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("ThereIsNewAskForHelp", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.AskForHelp, arMessage, enMessage, "");
            data.Properties.Add("UserId", userId);
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationForNotifyUserForNewOffer(List<long> userIds)
        {
            var enMessage = _localizationSource.GetString("SendNewOfferToUser", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("SendNewOfferToUser", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.NewOffer, arMessage, enMessage, "");
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationForOtherCompaniesForPossibleRequest(List<long> userIds, long requestId)
        {
            var enMessage = _localizationSource.GetString("ThereIsNewPossibleClient", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("ThereIsNewPossibleClient", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.PossibleClient, arMessage, enMessage, "");
            data.Properties.Add("RequestId", requestId);
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationForSelectedCompanies(List<long> userIds, long requestId)
        {
            var enMessage = _localizationSource.GetString("SendNewRequestFromAdminToCompany", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("SendNewRequestFromAdminToCompany", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.NewRequest, arMessage, enMessage, "");
            data.Properties.Add("RequestId", requestId);
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }

        public async Task SendNotificationToCompanyOrCompanyBranchForSelectedOfferByUser(List<long> userIds, Guid offerId)
        {
            var enMessage = _localizationSource.GetString("SendNewRequestFromAdminToCompany", CultureInfo.GetCultureInfo("en"));
            var arMessage = _localizationSource.GetString("SendNewRequestFromAdminToCompany", CultureInfo.GetCultureInfo("ar"));
            var data = new TypedMessageNotificationData(NotificationType.NewRequest, arMessage, enMessage, "");
            data.Properties.Add("OfferId", offerId);
            await _InotificationService.NotifyUsersAsync(data, userIds.ToArray(), true, true);
        }
    }
}

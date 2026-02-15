using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.NotificationSender
{
    public interface INotificationSender : ITransientDependency
    {
        Task SendNotificationForSelectedCompanies(List<long> userIds, long requestId);
        Task SendNotificationForNotifyUserForNewOffer(List<long> userIds);
        Task SendNotificationForCostumerServiceForAskHelp(List<long> userIds, long userId);
        Task SendNotificationToCompanyOrCompanyBranchForSelectedOfferByUser(List<long> userIds, Guid offerId);
        Task SendNotificationForCompanyForApproved(List<long> userIds);
        Task SendNotificationForCompanyForApproveUpdate(List<long> userIds);
        Task SendNotificationForOtherCompaniesForPossibleRequest(List<long> userIds, long requestId);
        Task SendNotificationForAdminToNoticHimAboutRequestToUpdateCompany(List<long> userIds, int parentCompanyId);
    }
}

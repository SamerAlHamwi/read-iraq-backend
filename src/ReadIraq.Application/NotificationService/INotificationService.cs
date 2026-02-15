using Abp.Dependency;
using System.Threading.Tasks;

namespace ReadIraq.NotificationService
{
    public interface INotificationService : ITransientDependency
    {
        /// <summary>
        /// Notify user and his delegation about notification data
        /// </summary>
        //Task NotifyUserAndHisDelegationUsersAsync(TypedMessageNotificationData data, long userId);

        ///// <summary>
        ///// Notify users and their delegations about notification data
        ///// </summary>
        //Task NotifyUsersAndTheirDelegationUsersAsync(TypedMessageNotificationData data, List<long> userIds);

        /// <summary>
        /// Notify users about notification data
        /// </summary>
        Task NotifyUsersAsync(TypedMessageNotificationData data, long[] userIds, bool withNotify, bool forEmailToo = false);
    }
}

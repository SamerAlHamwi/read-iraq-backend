using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PushNotifications
{
    public interface IPushNotificationManager : IDomainService
    {
        Task<PushNotification> GetPushNotificationById(int pushNotificationId);
    }
}

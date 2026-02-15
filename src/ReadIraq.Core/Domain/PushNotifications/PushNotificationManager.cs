using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PushNotifications
{
    public class PushNotificationManager : DomainService, IPushNotificationManager
    {
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        public PushNotificationManager(IRepository<PushNotification> pushNotificationRepository)
        {
            _pushNotificationRepository = pushNotificationRepository;

        }
        public Task<PushNotification> GetPushNotificationById(int pushNotificationId)
        {
            return _pushNotificationRepository.GetAll().Include(x => x.Translations).FirstOrDefaultAsync(x => x.Id == pushNotificationId);
        }
    }
}

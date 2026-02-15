using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.PushNotifications
{
    public class PushNotificationTranslation : FullAuditedEntity, IEntityTranslation<PushNotification>
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public PushNotification Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using ReadIraq.CrudAppServiceBase;
using ReadIraq.PushNotifications.Dto;

namespace ReadIraq.PushNotifications
{
    public interface IPushNotificationAppService : IReadIraqAsyncCrudAppService<PushNotificationDetailsDto, int, LitePushNotificationDto, PagedPushNotificationResultRequestDto,
         CreatePushNotificationDto, UpdatePushNotificationDto>
    {
    }
}

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Notifications.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Notifications
{
    public interface IAppNotificationAppService : IAsyncCrudAppService<AppNotificationDto, Guid, PagedAndSortedResultRequestDto, CreateAppNotificationDto, AppNotificationDto>
    {
        Task SendNotificationAsync(SendNotificationInput input);
        Task MarkAsReadAsync(EntityDto<Guid> input);
        Task ScheduleNotificationAsync(ScheduleNotificationInput input);
    }
}

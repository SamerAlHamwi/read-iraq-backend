using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Notifications.Dto;
using System;

namespace ReadIraq.Notifications
{
    public interface IAppNotificationAppService : IAsyncCrudAppService<AppNotificationDto, Guid, PagedAndSortedResultRequestDto, CreateAppNotificationDto, AppNotificationDto>
    {
    }
}

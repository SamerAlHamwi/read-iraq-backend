using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Notifications;
using ReadIraq.Notifications.Dto;
using System;

namespace ReadIraq.Notifications
{
    [AbpAuthorize]
    public class AppNotificationAppService : ReadIraqAsyncCrudAppService<AppNotification, AppNotificationDto, Guid, AppNotificationDto, PagedAndSortedResultRequestDto, CreateAppNotificationDto, AppNotificationDto>, IAppNotificationAppService
    {
        public AppNotificationAppService(IRepository<AppNotification, Guid> repository)
            : base(repository)
        {
        }
    }
}

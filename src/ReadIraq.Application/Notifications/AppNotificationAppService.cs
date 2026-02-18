using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Notifications;
using ReadIraq.NotificationService;
using ReadIraq.Notifications.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Notifications
{
    [AbpAuthorize]
    public class AppNotificationAppService : ReadIraqAsyncCrudAppService<AppNotification, AppNotificationDto, Guid, AppNotificationDto, PagedAndSortedResultRequestDto, CreateAppNotificationDto, AppNotificationDto>, IAppNotificationAppService
    {
        private readonly INotificationService _notificationService;
        private readonly IRepository<User, long> _userRepository;

        public AppNotificationAppService(
            IRepository<AppNotification, Guid> repository,
            INotificationService notificationService,
            IRepository<User, long> userRepository)
            : base(repository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task SendNotificationAsync(SendNotificationInput input)
        {
            var data = new TypedMessageNotificationData(NotificationType.General, input.Title, input.Title, input.Body);

            long[] targetUserIds;

            if (input.Filter.All)
            {
                targetUserIds = await _userRepository.GetAll().Select(x => x.Id).ToArrayAsync();
            }
            else if (input.Filter.UserIds != null && input.Filter.UserIds.Any())
            {
                targetUserIds = input.Filter.UserIds.ToArray();
            }
            else
            {
                // Other filters logic would go here
                targetUserIds = Array.Empty<long>();
            }

            if (targetUserIds.Any())
            {
                await _notificationService.NotifyUsersAsync(data, targetUserIds, true);
            }
        }

        public async Task MarkAsReadAsync(EntityDto<Guid> input)
        {
            var notification = await Repository.GetAsync(input.Id);
            if (notification == null)
            {
                throw new UserFriendlyException(L("ObjectWasNotFound", L("Notification")));
            }

            if (notification.UserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCannotDoThisAction"));
            }

            notification.IsRead = true;
            await Repository.UpdateAsync(notification);
        }

        public async Task ScheduleNotificationAsync(ScheduleNotificationInput input)
        {
            var notification = new AppNotification
            {
                Title = input.Title,
                Body = input.Body,
                ScheduledAt = input.ScheduledTime,
                IsRead = false,
                Type = "Scheduled"
            };

            await Repository.InsertAsync(notification);
        }

        protected override IQueryable<AppNotification> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Where(x => x.UserId == AbpSession.GetUserId() || x.UserId == null);
        }
    }
}

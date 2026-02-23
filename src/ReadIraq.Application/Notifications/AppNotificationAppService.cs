using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReadIraq.Authorization.Users;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Notifications;
using ReadIraq.NotificationService;
using ReadIraq.Notifications.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

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
            var data = new TypedMessageNotificationData(
                input.Type,
                input.Title, // Assuming these are passed as single strings for broadcast or we need to handle multi-lang input
                input.Title,
                input.Body,
                input.Body,
                null);

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
                targetUserIds = Array.Empty<long>();
            }

            if (targetUserIds.Any())
            {
                await _notificationService.NotifyUsersAsync(data, targetUserIds, true);
            }
        }

        public async Task MarkAsReadAsync(EntityDto<Guid> input)
        {
            await _notificationService.MarkAsReadAsync(input.Id);
        }

        public async Task DeleteNotificationAsync(EntityDto<Guid> input)
        {
            await _notificationService.DeleteNotificationAsync(input.Id);
        }

        public override async Task<PagedResultDto<AppNotificationDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var query = CreateFilteredQuery(input);
            var totalCount = await query.CountAsync();
            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var list = await query.ToListAsync();

            var lang = await SettingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, AbpSession.TenantId, AbpSession.GetUserId());
            var isArabic = lang != null && lang.ToUpper().Contains("AR");

            var dtos = list.Select(x => {
                var dto = MapToEntityDto(x);
                dto.Title = GetLocalizedValue(x.Title, isArabic);
                dto.Body = GetLocalizedValue(x.Body, isArabic);
                dto.CreatedAt = x.CreationTime;
                return dto;
            }).ToList();

            return new PagedResultDto<AppNotificationDto>(totalCount, dtos);
        }

        private string GetLocalizedValue(string json, bool isArabic)
        {
            if (string.IsNullOrEmpty(json)) return "";
            try {
                if (!json.Trim().StartsWith("{")) return json;
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                if (isArabic && values.ContainsKey("ar")) return values["ar"];
                if (values.ContainsKey("en")) return values["en"];
                return values.Values.FirstOrDefault() ?? "";
            } catch {
                return json;
            }
        }

        protected override IQueryable<AppNotification> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Where(x => x.UserId == AbpSession.GetUserId() || x.UserId == null);
        }
    }
}

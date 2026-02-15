using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.NotificationService;
using ReadIraq.PushNotifications.Dto;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PushNotifications
{
    [AbpAuthorize]
    public class PushNotificationAppService : ReadIraqAsyncCrudAppService<PushNotification, PushNotificationDetailsDto, int, LitePushNotificationDto,
        PagedPushNotificationResultRequestDto, CreatePushNotificationDto, UpdatePushNotificationDto>,
        IPushNotificationAppService
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager _userManager;
        private readonly IPushNotificationManager _pushNotificationManager;
        private readonly ILocalizationSource _localizationSource;
        private readonly ISettingManager _settingManager;
        private readonly IAbpSession _session;
        private readonly IMapper _mapper;
        /// <summary>
        ///  PushNotification AppService
        /// </summary>
        public PushNotificationAppService(IRepository<PushNotification> repository,
            INotificationService notificationService,
             UserManager userManager,
            IPushNotificationManager pushNotificationManager,
            ILocalizationManager localizationManager,
            ISettingManager settingManager,
            IAbpSession session,
            IMapper mapper)
         : base(repository)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _pushNotificationManager = pushNotificationManager;
            _localizationSource = localizationManager.GetSource(ReadIraqConsts.LocalizationSourceName);
            _settingManager = settingManager;
            _session = session;
            _mapper = mapper;
        }

        [ApiExplorerSettings(IgnoreApi = false)]
        public override async Task<PushNotificationDetailsDto> CreateAsync(CreatePushNotificationDto input)
        {

            CheckCreatePermission();
            var pushNotification = ObjectMapper.Map<PushNotification>(input);
            pushNotification.CreationTime = DateTime.UtcNow;
            await Repository.InsertAsync(pushNotification);
            UnitOfWorkManager.Current.SaveChanges();
            var arMessage = pushNotification.Translations.Where(x => x.Language == "ar").Select(x => x.Message).FirstOrDefault();
            var enMessage = pushNotification.Translations.Where(x => x.Language == "en").Select(x => x.Message).FirstOrDefault();
            var data = new TypedMessageNotificationData(NotificationType.PushNotification, arMessage, enMessage, "");
            var userIds = await GetUserIdsNotificationDestination(input.Destination);
            await _notificationService.NotifyUsersAsync(data, userIds, true);
            return MapToEntityDto(pushNotification);


        }

        [ApiExplorerSettings(IgnoreApi = false)]
        public override async Task<PagedResultDto<LitePushNotificationDto>> GetAllAsync(PagedPushNotificationResultRequestDto input)
        {
            var lang = await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, _session.TenantId, (long)AbpSession.UserId);

            var isArabic = lang.ToUpper().Contains("AR");

            var result = await base.GetAllAsync(input);
            foreach (var item in result.Items)
            {
                //   item.DestinationText = item.Destination.ToString();
                item.DestinationText = _localizationSource.GetString(item.Destination.ToString(), isArabic ?
                          CultureInfo.GetCultureInfo("ar-SY") :
                          CultureInfo.GetCultureInfo("en"));
                item.ArTitle = _localizationSource.GetString(NotificationType.PushNotification.ToString(), CultureInfo.GetCultureInfo("ar"));
                item.EnTitle = _localizationSource.GetString(NotificationType.PushNotification.ToString(), CultureInfo.GetCultureInfo("en"));

            }
            return result;
        }

        protected override IQueryable<PushNotification> CreateFilteredQuery(PagedPushNotificationResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Where(x => !x.IsDeleted);
            if (!input.Keyword.IsNullOrEmpty())
                data = data.Where(x => x.Translations.Where(x => x.Message.Contains(input.Keyword)).Any());
            if (input.Destination is not null)
                data = data.Where(x => x.Destination == input.Destination);
            data = data.Include(x => x.Translations);
            return data;
        }
        /// <summary>
        /// Sorting Filtered Posts
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<PushNotification> ApplySorting(IQueryable<PushNotification> query, PagedPushNotificationResultRequestDto input)
        {
            return query.OrderBy(r => r.CreationTime);
        }
        private async Task<long[]> GetUserIdsNotificationDestination(TopicType topic)
        {

            long[] UserIds = new long[] { };
            switch (topic)
            {
                case TopicType.All:
                    UserIds = _userManager.Users.Where(x => !x.IsDeleted).Select(x => x.Id).ToArray();
                    break;
                case TopicType.Admin:
                    UserIds = _userManager.Users.Where(x => x.Type == UserType.Admin).Select(x => x.Id).ToArray();
                    break;
                case TopicType.BasicUser:
                    UserIds = _userManager.Users.Where(x => x.Type == UserType.BasicUser).Select(x => x.Id).ToArray();
                    break;
            }
            return UserIds;

        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public override async Task<PushNotificationDetailsDto> UpdateAsync(UpdatePushNotificationDto input)
        {
            return new PushNotificationDetailsDto();
        }
        public async Task<bool> ResendPushNotification(int pushNotificationId)
        {

            CheckCreatePermission();
            var oldPushNotification = await _pushNotificationManager.GetPushNotificationById(pushNotificationId);
            if (oldPushNotification is not null)
            {
                var createnew = _mapper.Map<PushNotification, CreatePushNotificationDto>(oldPushNotification);
                try
                {
                    await CreateAsync(createnew);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new UserFriendlyException(ex.Message);
                }
            }
            throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Entity));
        }
    }
}

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

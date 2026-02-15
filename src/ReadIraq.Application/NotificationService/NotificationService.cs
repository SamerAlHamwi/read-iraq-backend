using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using static ReadIraq.Enums.Enum;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ReadIraq.NotificationService
{
	/// <summary>
	/// 
	/// </summary>
	public class NotificationService : INotificationService
	{
		private readonly IConfiguration _configuration;
		private readonly ISettingManager _settingManager;
		private readonly IAbpSession _session;
		private readonly ILocalizationSource _localizationSource;
		private readonly UserManager _userManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		/// <summary>
		/// 
		/// </summary>
		public ILogger Logger { get; set; }

		private readonly INotificationPublisher _notificationPublisher;
		//private readonly IDelegationManager _delegationManager;
		/// <summary>
		/// NotificationService
		/// </summary>
		/// <param name="notificationPublisher"></param>
		/// <param name="settingManager"></param>
		/// <param name="session"></param>
		/// <param name="configuration"></param>
		/// <param name="localizationManager"></param>
		/// <param name="userManager"></param>
		/// <param name="unitOfWorkManager"></param>

		public NotificationService(INotificationPublisher notificationPublisher,
			//IDelegationManager delegationManager,
			ISettingManager settingManager,
			IAbpSession session,
			IConfiguration configuration,
			ILocalizationManager localizationManager,
			UserManager userManager,
			IUnitOfWorkManager unitOfWorkManager)
		{
			_notificationPublisher = notificationPublisher;
			//_delegationManager = delegationManager;
			_settingManager = settingManager;
			_session = session;
			_configuration = configuration;
			_localizationSource = localizationManager.GetSource(ReadIraqConsts.LocalizationSourceName);
			_userManager = userManager;
			_unitOfWorkManager = unitOfWorkManager;
			InitializeFirebaseApp();

		}
		private FirebaseApp InitializeFirebaseApp()
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "FirebaseConfig", "movers-26fb9-firebase-adminsdk-uvgeb-30e0a931c9.json");
			GoogleCredential googleCredential = GoogleCredential.FromFile(filePath);

			if (FirebaseApp.DefaultInstance == null)
			{
				return FirebaseApp.Create(new AppOptions
				{
					Credential = googleCredential,
					ProjectId = "movers-26fb9"
				});
			}

			return FirebaseApp.DefaultInstance;
		}
		//public async Task NotifyUserAndHisDelegationUsersAsync(TypedMessageNotificationData data, long userId)
		//{
		//    var userIds = (await _delegationManager.GetEffectiveDelegationsFromUserAsync(userId))
		//        .Select(x => x.DelegatedToUserId).ToList();
		//    userIds.Add(userId);

		//    await NotifyUsersAsync(data, userIds.ToArray());
		//}

		//public async Task NotifyUsersAndTheirDelegationUsersAsync(TypedMessageNotificationData data, List<long> userIds)
		//{
		//    var notifiedUserIds = userIds.ToList();

		//    foreach (var userId in userIds)
		//    {
		//        notifiedUserIds.AddRange((await _delegationManager.GetEffectiveDelegationsFromUserAsync(userId))
		//            .Select(x => x.DelegatedToUserId).ToList());
		//    }

		//    await NotifyUsersAsync(data, notifiedUserIds.Distinct().ToArray());
		//}
		/// <summary>
		/// Notify Users
		/// </summary>
		/// <param name="data"></param>
		/// <param name="userIds"></param>
		/// <returns></returns>
		public async Task NotifyUsersAsync(TypedMessageNotificationData data, long[] userIds, bool withNotify, bool forEmailToo = false)
		{
			var userIdentifiers = userIds.Select(x =>
				new UserIdentifier(MultiTenancyConsts.DefaultTenantId, x)).ToArray();

			var notificationName = data.NotificationType.ToString();

			await _notificationPublisher.PublishAsync(notificationName, data, userIds: userIdentifiers);

			await SendPushNotification(data, userIds, withNotify: true, forEmailToo);

		}


		private async Task SendPushNotification(TypedMessageNotificationData data, long[] userIds, bool withNotify, bool forEmailToo = false)
		{
			var fcmApiUri = new Uri(_configuration["FCM:ApiUrl"]);
			var fcmAppId = _configuration["FCM:AppId"];
			var fcmSenderId = _configuration["FCM:SenderId"];
			using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
			{
				var users = await _userManager.Users.AsNoTracking().Where(x => userIds.Contains(x.Id)).Select(x => new { x.Id, x.FcmToken, x.EmailAddress, x.IsEmailConfirmed }).ToListAsync();
				foreach (var userId in userIds)
				{
					string result = null;
					var user = users.Where(x => x.Id == userId).FirstOrDefault();
					if (user is null)
						continue;
					var lang = await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, _session.TenantId, userId);
					var isArabic = lang.ToUpper().Contains("AR");
					var title = _localizationSource.GetString(data.NotificationType.ToString(), isArabic ?
						CultureInfo.GetCultureInfo("ar") :
						CultureInfo.GetCultureInfo("en"));
					var notificationProperties = data.Properties;
					var message = isArabic ? data.ArMessage : data.EnMessage;
				
					if (string.IsNullOrEmpty(user.FcmToken))
						continue;


					var extData = new Dictionary<string, string>
							{
								{"time", DateTime.Now.ToString("dd-MM-yyyy HH:mm")},
								{"type", ((byte) data.NotificationType).ToString()},
							};
					var notification = new FirebaseAdmin.Messaging.Message
					{
						Token = user.FcmToken,
						Notification = new FirebaseAdmin.Messaging.Notification
						{
							Title = title,
							Body = message,
						},
						Data = extData,
						Apns = new ApnsConfig
						{
							Aps = new Aps
							{
								Sound = "default",
								ContentAvailable = true
							}
						},
					};
					if (withNotify)
					{

						try
						{
							// Send the push notification
							var resonpse = await FirebaseMessaging.DefaultInstance.SendAsync(notification);

							Logger.Info($"Successfully sent notification to {user.FcmToken}, result: {result}");

							result = resonpse;
						}
						catch
						{
							throw new Exception($"PushNotification not sent to ({result}).");
						}
					}
					if (!withNotify)
					{
						notification.Apns = new ApnsConfig
						{
							Aps = new Aps
							{
								Sound = "",
								ContentAvailable = true
							}
						};
						try
						{
							var resonpse = await FirebaseMessaging.DefaultInstance.SendAsync(notification);

							Logger.Info($"Successfully sent notification to {user.FcmToken}, result: {result}");

							result = resonpse;

						}
						catch
						{
							Logger.Error($"PushNotification not sent to ({result}).");
						}
					}
				}
			}
		}
	}
}

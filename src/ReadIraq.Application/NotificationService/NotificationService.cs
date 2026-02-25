using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.MultiTenancy;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using Castle.Core.Logging;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Notifications;
using static ReadIraq.Enums.Enum;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ReadIraq.NotificationService
{
	public class NotificationService : INotificationService
	{
		private readonly IConfiguration _configuration;
		private readonly ISettingManager _settingManager;
		private readonly IAbpSession _session;
		private readonly ILocalizationSource _localizationSource;
		private readonly UserManager _userManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<AppNotification, Guid> _notificationRepository;

		public ILogger Logger { get; set; }

		private readonly INotificationPublisher _notificationPublisher;

		public NotificationService(
			INotificationPublisher notificationPublisher,
			ISettingManager settingManager,
			IAbpSession session,
			IConfiguration configuration,
			ILocalizationManager localizationManager,
			UserManager userManager,
			IUnitOfWorkManager unitOfWorkManager,
			IRepository<AppNotification, Guid> notificationRepository)
		{
			_notificationPublisher = notificationPublisher;
			_settingManager = settingManager;
			_session = session;
			_configuration = configuration;
			_localizationSource = localizationManager.GetSource(ReadIraqConsts.LocalizationSourceName);
			_userManager = userManager;
			_unitOfWorkManager = unitOfWorkManager;
			_notificationRepository = notificationRepository;
			InitializeFirebaseApp();
		}

		private FirebaseApp InitializeFirebaseApp()
		{
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "FirebaseConfig", "iqra-iraq-project-firebase-adminsdk-fbsvc-d987751ecf.json");

			if (!File.Exists(filePath))
			{
				Logger.Warn($"Firebase config file not found at {filePath}");
				return null;
			}

			GoogleCredential googleCredential = GoogleCredential.FromFile(filePath);

			if (FirebaseApp.DefaultInstance == null)
			{
				return FirebaseApp.Create(new AppOptions
				{
					Credential = googleCredential,
					ProjectId = "iqra-iraq-project"
				});
			}

			return FirebaseApp.DefaultInstance;
		}

		public async Task NotifyUsersAsync(TypedMessageNotificationData data, long[] userIds, bool withPush, bool forEmailToo = false, DateTime? scheduledAtUtc = null)
		{
			foreach (var userId in userIds)
			{
				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null) continue;

				// Create persistent record for In-App (always saved)
				var notification = new AppNotification
				{
					UserId = userId,
					Type = data.NotificationType,
					Title = JsonConvert.SerializeObject(new { en = data.EnTitle, ar = data.ArTitle }),
					Body = JsonConvert.SerializeObject(new { en = data.EnMessage, ar = data.ArMessage }),
					Data = data.AdditionalValue,
					Priority = ReadIraq.Enums.Enum.NotificationPriority.DEFAULT,
					Channel = NotificationChannel.IN_APP,
					IsRead = false,
					ScheduledAtUtc = scheduledAtUtc,
					DeliveryStatus = NotificationDeliveryStatus.PENDING
				};

				await _notificationRepository.InsertAsync(notification);

				// Respect User Preferences for Push
				bool shouldSendPush = withPush && user.PushEnabled;

				if (shouldSendPush && (scheduledAtUtc == null || scheduledAtUtc <= DateTime.UtcNow))
				{
					await _unitOfWorkManager.Current.SaveChangesAsync();
					await SendPushNotificationInternal(notification, data);
				}

				// Email logic would be triggered here based on user.EmailEnabled
				if (user.EmailEnabled || forEmailToo)
				{
					// TODO: Implement Email Sending Logic
				}
			}
		}

		public async Task MarkAsReadAsync(Guid notificationId)
		{
			var notification = await _notificationRepository.GetAsync(notificationId);
			if (notification.UserId != _session.UserId)
			{
				throw new UserFriendlyException(_localizationSource.GetString("YouCanOnlyMarkYourOwnNotificationsAsRead"));
			}
			notification.IsRead = true;
			await _notificationRepository.UpdateAsync(notification);
		}

		public async Task DeleteNotificationAsync(Guid notificationId)
		{
			var notification = await _notificationRepository.GetAsync(notificationId);
			if (notification.UserId != _session.UserId)
			{
				throw new UserFriendlyException(_localizationSource.GetString("YouCanOnlyDeleteYourOwnNotifications"));
			}
			await _notificationRepository.DeleteAsync(notification);
		}

		#region Specific Notification Methods

		public async Task NotifyDailyStudyReminderAsync(long userId, Guid lessonId, string lessonName, Guid subjectId)
		{
			var data = CreateLocalizedData(
				NotificationType.DAILY_STUDY_REMINDER,
				JsonConvert.SerializeObject(new { lessonId, lessonName, subjectId }),
				lessonName
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifyNewLessonUploadedAsync(long[] userIds, Guid lessonId, string lessonName, string teacherName, Guid subjectId, Guid teacherId)
		{
			var data = CreateLocalizedData(
				NotificationType.NEW_LESSON_UPLOADED,
				JsonConvert.SerializeObject(new { lessonId, teacherId, subjectId }),
				teacherName, lessonName
			);
			await NotifyUsersAsync(data, userIds, true);
		}

		public async Task NotifyQuizReminderAsync(long userId, Guid quizId, Guid lessonId, string lessonName)
		{
			var data = CreateLocalizedData(
				NotificationType.QUIZ_REMINDER_TO_TAKE,
				JsonConvert.SerializeObject(new { quizId, lessonId }),
				lessonName
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifyQuizPassedHighScoreAsync(long userId, Guid quizId, Guid lessonId, string lessonName, int score)
		{
			var data = CreateLocalizedData(
				NotificationType.QUIZ_PASSED_HIGH_SCORE,
				JsonConvert.SerializeObject(new { quizId, lessonId, score }),
				score, lessonName
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifyWeeklyProgressReportAsync(long userId, int minutesStudied, int lessonsCompleted)
		{
			var data = CreateLocalizedData(
				NotificationType.WEEKLY_PROGRESS_REPORT,
				null,
				minutesStudied, lessonsCompleted
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifySubscriptionExpiringAsync(long userId, int days)
		{
			var data = CreateLocalizedData(
				NotificationType.SUBSCRIPTION_EXPIRING,
				null,
				days
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifyTeacherReplyAsync(long userId, Guid lessonId)
		{
			var data = CreateLocalizedData(
				NotificationType.TEACHER_REPLY,
				JsonConvert.SerializeObject(new { lessonId })
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifyStreakBrokenAsync(long userId, int nDays, string lessonName, Guid lessonId)
		{
			var data = CreateLocalizedData(
				NotificationType.STREAK_BROKEN,
				JsonConvert.SerializeObject(new { lessonId }),
				nDays, lessonName
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		public async Task NotifySystemMaintenanceAsync(long[] userIds, string date, string start, string end)
		{
			var data = CreateLocalizedData(
				NotificationType.SYSTEM_MAINTENANCE,
				null,
				date, start, end
			);
			await NotifyUsersAsync(data, userIds, true);
		}

		public async Task NotifyContentRecommendationAsync(long userId, string lessonName, Guid lessonId)
		{
			var data = CreateLocalizedData(
				NotificationType.CONTENT_RECOMMENDATION,
				JsonConvert.SerializeObject(new { lessonId }),
				lessonName
			);
			await NotifyUsersAsync(data, new[] { userId }, true);
		}

		#endregion

		private TypedMessageNotificationData CreateLocalizedData(NotificationType type, string additionalValue, params object[] args)
		{
			var arCulture = CultureInfo.GetCultureInfo("ar");
			var enCulture = CultureInfo.GetCultureInfo("en");

			var titleKey = $"Notification_{type}_Title";
			var bodyKey = $"Notification_{type}_Body";

			return new TypedMessageNotificationData(
				type,
				_localizationSource.GetString(titleKey, arCulture),
				_localizationSource.GetString(titleKey, enCulture),
				_localizationSource.GetString(bodyKey, arCulture, args),
				_localizationSource.GetString(bodyKey, enCulture, args),
				additionalValue
			);
		}

		private async Task SendPushNotificationInternal(AppNotification appNotification, TypedMessageNotificationData data)
		{
			try
			{
				var user = await _userManager.GetUserByIdAsync(appNotification.UserId.Value);
				if (string.IsNullOrEmpty(user.FcmToken))
				{
					appNotification.DeliveryStatus = NotificationDeliveryStatus.FAILED;
					appNotification.Meta = JsonConvert.SerializeObject(new { error = "No FCM token found for user." });
					await _notificationRepository.UpdateAsync(appNotification);
					return;
				}

				var lang = await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, _session.TenantId, user.Id);
				var isArabic = lang != null && lang.ToUpper().Contains("AR");
				
				var title = isArabic ? data.ArTitle : data.EnTitle;
				var message = isArabic ? data.ArMessage : data.EnMessage;

				var extData = new Dictionary<string, string>
				{
					{"id", appNotification.Id.ToString()},
					{"time", DateTime.UtcNow.ToString("O")},
					{"type", ((byte) data.NotificationType).ToString()},
					{"additionalData", data.AdditionalValue ?? ""}
				};

				var fcmMessage = new FirebaseAdmin.Messaging.Message
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
					Android = new AndroidConfig
					{
						Priority = Priority.High,
						Notification = new AndroidNotification
						{
							Sound = "default",
							ClickAction = "FLUTTER_NOTIFICATION_CLICK"
						}
					}
				};

				var response = await FirebaseMessaging.DefaultInstance.SendAsync(fcmMessage);

				appNotification.FCMMessageId = response;
				appNotification.DeliveryStatus = NotificationDeliveryStatus.SENT;
				appNotification.SentAtUtc = DateTime.UtcNow;
				await _notificationRepository.UpdateAsync(appNotification);

				Logger.Info($"Successfully sent notification {appNotification.Id} to {user.FcmToken}, FCM ID: {response}");
			}
			catch (Exception ex)
			{
				appNotification.DeliveryStatus = NotificationDeliveryStatus.FAILED;
				appNotification.Meta = JsonConvert.SerializeObject(new { error = ex.Message, stackTrace = ex.StackTrace });
				await _notificationRepository.UpdateAsync(appNotification);
				Logger.Error($"Failed to send notification {appNotification.Id}: {ex.Message}", ex);
			}
		}
	}
}

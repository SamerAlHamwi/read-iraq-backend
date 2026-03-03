using Abp.Dependency;
using System;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.NotificationService
{
    public interface INotificationService : ITransientDependency
    {
        Task NotifyUsersAsync(TypedMessageNotificationData data, long[] userIds, bool withPush, bool forEmailToo = false, DateTime? scheduledAtUtc = null);
        Task MarkAsReadAsync(Guid notificationId);
        Task DeleteNotificationAsync(Guid notificationId);

        // Core Notification Types
        Task NotifyDailyStudyReminderAsync(long userId, Guid lessonId, string lessonName, Guid subjectId);
        Task NotifyNewLessonUploadedAsync(long[] userIds, Guid lessonId, string lessonName, string teacherName, Guid subjectId, Guid teacherId);
        Task NotifyQuizReminderAsync(long userId, Guid quizId, Guid lessonId, string lessonName);
        Task NotifyQuizPassedHighScoreAsync(long userId, Guid quizId, Guid lessonId, string lessonName, int score);
        Task NotifyWeeklyProgressReportAsync(long userId, int minutesStudied, int lessonsCompleted);
        Task NotifyTeacherReplyAsync(long userId, Guid lessonId);
        Task NotifyStreakBrokenAsync(long userId, int nDays, string lessonName, Guid lessonId);
        Task NotifySystemMaintenanceAsync(long[] userIds, string date, string start, string end);
        Task NotifyContentRecommendationAsync(long userId, string lessonName, Guid lessonId);
    }
}

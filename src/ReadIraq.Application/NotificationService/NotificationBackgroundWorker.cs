using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Domain.UserSessionProgresses;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.NotificationService
{
    public class NotificationBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;
        private readonly IRepository<LessonSession, Guid> _lessonSessionRepository;
        private readonly IRepository<UserSessionProgress, Guid> _progressRepository;
        private readonly IRepository<Quiz, Guid> _quizRepository;
        private readonly IRepository<QuizAttempt, Guid> _quizAttemptRepository;
        private readonly INotificationService _notificationService;

        public NotificationBackgroundWorker(
            AbpTimer timer,
            IRepository<User, long> userRepository,
            IRepository<Enrollment, Guid> enrollmentRepository,
            IRepository<Subscription, Guid> subscriptionRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IRepository<UserSessionProgress, Guid> progressRepository,
            IRepository<Quiz, Guid> quizRepository,
            IRepository<QuizAttempt, Guid> quizAttemptRepository,
            INotificationService notificationService)
            : base(timer)
        {
            _userRepository = userRepository;
            _enrollmentRepository = enrollmentRepository;
            _subscriptionRepository = subscriptionRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _progressRepository = progressRepository;
            _quizRepository = quizRepository;
            _quizAttemptRepository = quizAttemptRepository;
            _notificationService = notificationService;

            // Run every 15 minutes to check for scheduled/daily notifications
            Timer.Period = 15 * 60 * 1000;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            AsyncHelper.RunSync(() => ProcessDailyReminders());
            AsyncHelper.RunSync(() => ProcessSubscriptionExpirations());
            AsyncHelper.RunSync(() => ProcessStreakBroken());
            AsyncHelper.RunSync(() => ProcessQuizReminders());

            // Weekly report every Sunday at 10 AM (example)
            var now = DateTime.UtcNow;
            if (now.DayOfWeek == DayOfWeek.Sunday && now.Hour == 10 && now.Minute < 15)
            {
                AsyncHelper.RunSync(() => ProcessWeeklyReports());
            }
        }

        private async Task ProcessDailyReminders()
        {
            var now = DateTime.UtcNow;
            var currentTime = now.TimeOfDay;

            var usersToRemind = await _userRepository.GetAll()
                .Where(u => u.DailyReminderEnabled && u.DailyReminderTime.HasValue)
                .Where(u => u.DailyReminderTime.Value <= currentTime && u.DailyReminderTime.Value > currentTime.Add(TimeSpan.FromMinutes(-15)))
                .ToListAsync();

            foreach (var user in usersToRemind)
            {
                var enrollment = await _enrollmentRepository.GetAll()
                    .Include(x => x.Subject)
                    .Where(x => x.UserId == user.Id && x.ProgressPercent < 100)
                    .OrderBy(x => x.ProgressPercent)
                    .FirstOrDefaultAsync();

                if (enrollment != null)
                {
                    var lesson = await _lessonSessionRepository.GetAll()
                        .Where(x => x.SubjectId == enrollment.SubjectId)
                        .OrderBy(x => x.Order)
                        .FirstOrDefaultAsync();

                    if (lesson != null)
                    {
                        await _notificationService.NotifyDailyStudyReminderAsync(user.Id, lesson.Id, lesson.Title, enrollment.SubjectId);
                    }
                }
            }
        }

        private async Task ProcessSubscriptionExpirations()
        {
            var targetDate = DateTime.UtcNow.Date.AddDays(3);

            var expiringSubscriptions = await _subscriptionRepository.GetAll()
                .Where(s => s.IsActive && s.ExpiresAt.Date == targetDate)
                .ToListAsync();

            foreach (var sub in expiringSubscriptions)
            {
                await _notificationService.NotifySubscriptionExpiringAsync(sub.UserId, 3);
            }
        }

        private async Task ProcessStreakBroken()
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);

            var usersWithBrokenStreaks = await _userRepository.GetAll()
                .Where(u => u.LastStudiedAt.HasValue && u.LastStudiedAt.Value < yesterday.Date && u.LastStudiedAt.Value >= yesterday.AddDays(-1).Date)
                .ToListAsync();

            foreach (var user in usersWithBrokenStreaks)
            {
                 var enrollment = await _enrollmentRepository.GetAll()
                    .Where(x => x.UserId == user.Id)
                    .OrderByDescending(x => x.ProgressPercent)
                    .FirstOrDefaultAsync();

                 if (enrollment != null)
                 {
                    var lesson = await _lessonSessionRepository.GetAll()
                        .Where(x => x.SubjectId == enrollment.SubjectId)
                        .OrderBy(x => x.Order)
                        .FirstOrDefaultAsync();

                    if (lesson != null)
                    {
                        await _notificationService.NotifyStreakBrokenAsync(user.Id, 5, lesson.Title, lesson.Id);
                    }
                 }
            }
        }

        private async Task ProcessQuizReminders()
        {
            var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);

            // Lessons completed ~24h ago
            var completedProgresses = await _progressRepository.GetAll()
                .Include(x => x.Session)
                .Where(x => x.IsCompleted && x.LastWatchedAt <= twentyFourHoursAgo && x.LastWatchedAt > twentyFourHoursAgo.AddMinutes(-15))
                .ToListAsync();

            foreach (var progress in completedProgresses)
            {
                var quiz = await _quizRepository.FirstOrDefaultAsync(q => q.SessionId == progress.SessionId);
                if (quiz != null)
                {
                    // Check if user has already attempted it
                    var attempt = await _quizAttemptRepository.FirstOrDefaultAsync(a => a.QuizId == quiz.Id && a.UserId == progress.UserId);
                    if (attempt == null)
                    {
                        await _notificationService.NotifyQuizReminderAsync(progress.UserId, quiz.Id, progress.SessionId, progress.Session.Title);
                    }
                }
            }
        }

        private async Task ProcessWeeklyReports()
        {
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var users = await _userRepository.GetAll().Where(u => u.IsActive).ToListAsync();

            foreach (var user in users)
            {
                // Calculate stats for the last 7 days
                var lessonsCompletedCount = await _progressRepository.CountAsync(p => p.UserId == user.Id && p.IsCompleted && p.LastWatchedAt >= lastWeek);

                // Simple assumption for minutes studied
                var totalSeconds = await _progressRepository.GetAll()
                    .Where(p => p.UserId == user.Id && p.LastWatchedAt >= lastWeek)
                    .SumAsync(p => p.WatchedSeconds);

                if (lessonsCompletedCount > 0 || totalSeconds > 0)
                {
                    await _notificationService.NotifyWeeklyProgressReportAsync(user.Id, (int)(totalSeconds / 60), lessonsCompletedCount);
                }
            }
        }
    }
}

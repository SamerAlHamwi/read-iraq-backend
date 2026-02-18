using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Users;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.UserSessionProgresses;
using ReadIraq.UserSessionProgresses.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.UserSessionProgresses
{
    [AbpAuthorize]
    public class UserSessionProgressAppService : ReadIraqAsyncCrudAppService<UserSessionProgress, UserSessionProgressDto, Guid, UserSessionProgressDto, PagedUserSessionProgressResultRequestDto, CreateUserSessionProgressDto, UpdateUserSessionProgressDto>, IUserSessionProgressAppService
    {
        private readonly IRepository<LessonSession, Guid> _sessionRepository;
        private readonly IRepository<User, long> _userRepository;

        public UserSessionProgressAppService(
            IRepository<UserSessionProgress, Guid> repository,
            IRepository<LessonSession, Guid> sessionRepository,
            IRepository<User, long> userRepository)
            : base(repository)
        {
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
        }

        protected override IQueryable<UserSessionProgress> CreateFilteredQuery(PagedUserSessionProgressResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.SessionId.HasValue, x => x.SessionId == input.SessionId.Value);
        }

        public async Task<UserProgressSummaryDto> GetSummaryAsync(long userId)
        {
            if (userId != AbpSession.GetUserId() && !PermissionChecker.IsGranted(PermissionNames.Pages_Users))
            {
                throw new Abp.UI.UserFriendlyException(L("InsufficientPermissions"));
            }

            var user = await _userRepository.GetAsync(userId);
            var completedSessionsCount = await Repository.CountAsync(x => x.UserId == userId && x.IsCompleted);
            var totalSessionsCount = await _sessionRepository.CountAsync(x => x.IsActive);

            return new UserProgressSummaryDto
            {
                TotalCompletedSessions = completedSessionsCount,
                OverallCompletionPercentage = totalSessionsCount > 0 ? (double)completedSessionsCount / totalSessionsCount * 100 : 0,
                TotalPoints = user.Points
            };
        }

        public async Task UpdateProgressAsync(UpdateUserSessionProgressDto input)
        {
            var userId = AbpSession.GetUserId();
            // User can only update their own progress
            var progress = await Repository.FirstOrDefaultAsync(x => x.UserId == userId && x.SessionId == input.SessionId);

            if (progress == null)
            {
                progress = new UserSessionProgress
                {
                    UserId = userId,
                    SessionId = input.SessionId,
                    WatchedSeconds = input.WatchedSeconds,
                    IsCompleted = input.IsCompleted,
                    LastWatchedAt = DateTime.Now
                };
                await Repository.InsertAsync(progress);
            }
            else
            {
                progress.WatchedSeconds = Math.Max(progress.WatchedSeconds, input.WatchedSeconds);
                if (!progress.IsCompleted)
                {
                    progress.IsCompleted = input.IsCompleted;
                }
                progress.LastWatchedAt = DateTime.Now;
                await Repository.UpdateAsync(progress);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<SubjectProgressDto> GetSubjectProgressAsync(Guid subjectId, long userId)
        {
            if (userId != AbpSession.GetUserId() && !PermissionChecker.IsGranted(PermissionNames.Pages_Users))
            {
                throw new Abp.UI.UserFriendlyException(L("InsufficientPermissions"));
            }

            var subjectSessions = _sessionRepository.GetAll().Where(x => x.SubjectId == subjectId && x.IsActive);
            var totalSessions = await subjectSessions.CountAsync();

            var subjectSessionIds = subjectSessions.Select(x => x.Id);
            var completedSessions = await Repository.CountAsync(x => x.UserId == userId && subjectSessionIds.Contains(x.SessionId) && x.IsCompleted);

            return new SubjectProgressDto
            {
                TotalSessions = totalSessions,
                CompletedSessions = completedSessions,
                ProgressPercentage = totalSessions > 0 ? (double)completedSessions / totalSessions * 100 : 0
            };
        }
    }
}

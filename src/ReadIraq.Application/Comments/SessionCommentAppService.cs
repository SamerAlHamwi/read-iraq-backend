using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Comments.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Comments;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Domain.Teachers;
using ReadIraq.NotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Comments
{
    [AbpAuthorize]
    public class SessionCommentAppService : ReadIraqAsyncCrudAppService<SessionComment, SessionCommentDto, Guid, SessionCommentDto, PagedSessionCommentResultRequestDto, CreateSessionCommentDto, UpdateSessionCommentDto>, ISessionCommentAppService
    {
        private readonly INotificationService _notificationService;
        private readonly IRepository<LessonSession, Guid> _lessonSessionRepository;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherProfileRepository;

        public SessionCommentAppService(
            IRepository<SessionComment, Guid> repository,
            INotificationService notificationService,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IRepository<Enrollment, Guid> enrollmentRepository,
            IRepository<TeacherProfile, Guid> teacherProfileRepository)
            : base(repository)
        {
            _notificationService = notificationService;
            _lessonSessionRepository = lessonSessionRepository;
            _enrollmentRepository = enrollmentRepository;
            _teacherProfileRepository = teacherProfileRepository;
        }

        protected override IQueryable<SessionComment> CreateFilteredQuery(PagedSessionCommentResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.User)
                .Include(x => x.Replies)
                    .ThenInclude(r => r.User)
                .Where(x => x.ParentCommentId == null)
                .WhereIf(input.LessonSessionId.HasValue, x => x.LessonSessionId == input.LessonSessionId.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Text.Contains(input.Keyword));
        }

        public override async Task<SessionCommentDto> CreateAsync(CreateSessionCommentDto input)
        {
            var userId = AbpSession.GetUserId();
            var session = await _lessonSessionRepository.GetAsync(input.LessonSessionId);

            bool isTeacher = await CheckIfTeacherAsync(session, userId);

            if (!isTeacher && !session.IsFree)
            {
                var isEnrolled = await _enrollmentRepository.GetAll().AnyAsync(x => x.UserId == userId && x.SubjectId == session.SubjectId);
                if (!isEnrolled)
                {
                    throw new UserFriendlyException("You must be enrolled in this subject to participate in the discussion.");
                }
            }

            var entity = MapToEntity(input);
            entity.UserId = userId;
            entity.IsByTeacher = isTeacher;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task<SessionCommentDto> ReplyAsync(Guid id, CreateSessionCommentDto input)
        {
            var parentComment = await Repository.FirstOrDefaultAsync(id);
            if (parentComment == null)
            {
                throw new UserFriendlyException(L("CommentNotFound"));
            }

            var userId = AbpSession.GetUserId();
            var session = await _lessonSessionRepository.GetAsync(parentComment.LessonSessionId);

            bool isTeacher = await CheckIfTeacherAsync(session, userId);

            if (!isTeacher && !session.IsFree)
            {
                var isEnrolled = await _enrollmentRepository.GetAll().AnyAsync(x => x.UserId == userId && x.SubjectId == session.SubjectId);
                if (!isEnrolled)
                {
                    throw new UserFriendlyException("You must be enrolled in this subject to participate in the discussion.");
                }
            }

            var reply = new SessionComment
            {
                LessonSessionId = parentComment.LessonSessionId,
                ParentCommentId = id,
                Text = input.Text,
                UserId = userId,
                IsByTeacher = isTeacher
            };

            await Repository.InsertAsync(reply);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Notify parent comment author if it's someone else replying
            if (parentComment.UserId != userId)
            {
                await _notificationService.NotifyTeacherReplyAsync(parentComment.UserId, parentComment.LessonSessionId);
            }

            return MapToEntityDto(reply);
        }

        private async Task<bool> CheckIfTeacherAsync(LessonSession session, long userId)
        {
            if (session == null) return false;

            var profile = await _teacherProfileRepository.FirstOrDefaultAsync(session.TeacherProfileId);
            return profile != null && profile.UserId == userId;
        }

        public override async Task<SessionCommentDto> UpdateAsync(UpdateSessionCommentDto input)
        {
            var entity = await Repository.GetAsync(input.Id);

            if (entity.UserId != AbpSession.GetUserId() && !IsAdmin())
            {
                throw new UserFriendlyException(L("OnlyAuthorOrAdminCanUpdateComment"));
            }

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public override async Task DeleteAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);

            if (entity.UserId != AbpSession.GetUserId() && !IsAdmin())
            {
                throw new UserFriendlyException(L("OnlyAuthorOrAdminCanDeleteComment"));
            }

            await Repository.DeleteAsync(input.Id);
        }

        private bool IsAdmin()
        {
            return PermissionChecker.IsGranted(Authorization.PermissionNames.Pages_Users);
        }

        protected override SessionCommentDto MapToEntityDto(SessionComment entity)
        {
            var dto = base.MapToEntityDto(entity);
            if (entity.User != null)
            {
                dto.UserName = entity.User.Name;
                dto.UserProfilePicture = entity.User.Avatar;
            }
            if (entity.Replies != null && entity.Replies.Any())
            {
                dto.Replies = entity.Replies.Select(MapToEntityDto).OrderBy(x => x.CreationTime).ToList();
            }
            return dto;
        }
    }
}

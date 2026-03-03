using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.UserSessionProgresses;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.SavedItems;
using static ReadIraq.Enums.Enum;
using System.Collections.Generic;
using ReadIraq.NotificationService;
using ReadIraq.Domain.Subjects;
using ReadIraq.Authorization.Users;
using System.Globalization;

namespace ReadIraq.LessonSessions
{
    [AbpAuthorize]
    public class LessonSessionAppService : ReadIraqAsyncCrudAppService<LessonSession, LessonSessionDto, Guid, LiteLessonSessionDto,
        PagedLessonSessionResultRequestDto, CreateLessonSessionDto, UpdateLessonSessionDto>,
        ILessonSessionAppService
    {
        private readonly IRepository<UserSessionProgress, Guid> _progressRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Attachment, long> _attachmentRepository;
        private readonly INotificationService _notificationService;
        private readonly IRepository<UserPreferredSubject, Guid> _userPreferredSubjectRepository;
        private readonly UserManager _userManager;
        private readonly IRepository<UserSavedItem, Guid> _userSavedItemRepository;

        public LessonSessionAppService(
            IRepository<LessonSession, Guid> repository,
            IRepository<UserSessionProgress, Guid> progressRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository,
            INotificationService notificationService,
            IRepository<UserPreferredSubject, Guid> userPreferredSubjectRepository,
            UserManager userManager,
            IRepository<UserSavedItem, Guid> userSavedItemRepository)
            : base(repository)
        {
            _progressRepository = progressRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _notificationService = notificationService;
            _userPreferredSubjectRepository = userPreferredSubjectRepository;
            _userManager = userManager;
            _userSavedItemRepository = userSavedItemRepository;
        }

        protected override IQueryable<LessonSession> CreateFilteredQuery(PagedLessonSessionResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Subject)
                .Include(x => x.Unit)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                    x => x.Title.Contains(input.Keyword) || x.Description.Contains(input.Keyword))
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId.Value)
                .WhereIf(input.UnitId.HasValue, x => x.UnitId == input.UnitId.Value)
                .WhereIf(input.IsFree.HasValue, x => x.IsFree == input.IsFree.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        public override async Task<LessonSessionDto> CreateAsync(CreateLessonSessionDto input)
        {
            var entity = MapToEntity(input);
            entity.IsActive = true;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ThumbnailAttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.ThumbnailAttachmentId, AttachmentRefType.LessonSessionThumbnail, entity.Id.ToString());
                entity.ThumbnailAttachmentId = input.ThumbnailAttachmentId;
            }

            if (input.VideoAttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.VideoAttachmentId, AttachmentRefType.LessonSessionVideo, entity.Id.ToString());
                entity.VideoAttachmentId = input.VideoAttachmentId;
            }

            if (input.AttachmentIds != null && input.AttachmentIds.Any())
            {
                foreach (var attachmentId in input.AttachmentIds)
                {
                    await _attachmentManager.CheckAndUpdateRefIdAsync(attachmentId, AttachmentRefType.LessonSessionOther, entity.Id.ToString());
                }
            }

            var lesson = MapToEntityDto(entity);

            // Trigger notification for users enrolled in this subject
            var enrolledUserIds = await _userPreferredSubjectRepository.GetAll()
                .Where(x => x.SubjectId == input.SubjectId)
                .Select(x => x.UserId)
                .ToArrayAsync();

            if (enrolledUserIds.Any())
            {
                var teacher = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
                await _notificationService.NotifyNewLessonUploadedAsync(
                    enrolledUserIds,
                    lesson.Id,
                    lesson.Title,
                    teacher?.Name ?? "Teacher",
                    input.SubjectId,
                    teacher?.Id != null ? Guid.Empty : Guid.Empty
                );
            }

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteLessonSessionDto>> GetAllAsync(PagedLessonSessionResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            var userId = AbpSession.UserId;

            foreach (var item in result.Items)
            {
                var entity = await Repository.GetAll()
                    .Include(x => x.Unit).ThenInclude(x => x.Name)
                    .FirstOrDefaultAsync(x => x.Id == item.Id);

                if (entity == null) continue;

                if (entity.Unit != null)
                {
                    var currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    item.UnitName = entity.Unit.Name?.FirstOrDefault(x => x.Code == currentLanguage)?.Name
                        ?? entity.Unit.Name?.FirstOrDefault()?.Name;
                }

                if (entity.ThumbnailAttachmentId.HasValue)
                {
                    var thumbnail = await _attachmentRepository.FirstOrDefaultAsync(entity.ThumbnailAttachmentId.Value);
                    if (thumbnail != null)
                    {
                        item.Thumbnail = ObjectMapper.Map<LiteAttachmentDto>(thumbnail);
                        item.Thumbnail.Url = _attachmentManager.GetUrl(thumbnail);
                        item.Thumbnail.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(thumbnail);
                    }
                }

                if (entity.VideoAttachmentId.HasValue)
                {
                    var video = await _attachmentRepository.FirstOrDefaultAsync(entity.VideoAttachmentId.Value);
                    if (video != null)
                    {
                        item.Video = ObjectMapper.Map<LiteAttachmentDto>(video);
                        item.Video.Url = _attachmentManager.GetUrl(video);
                    }
                }

                if (userId.HasValue)
                {
                    item.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == item.Id && x.ItemType == SavedItemType.Session);
                }
            }

            return result;
        }

        [AbpAllowAnonymous]
        public override async Task<LessonSessionDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                .Include(x => x.Subject).ThenInclude(x => x.Name)
                .Include(x => x.Unit).ThenInclude(x => x.Name)
                .Include(x => x.TeacherProfile)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("SessionNotFound"));
            }

            return await MapLessonSessionToDto(entity);
        }

        [AbpAllowAnonymous]
        public async Task<ListResultDto<LessonSessionDto>> GetByIdsAsync(GetLessonsByIdsInput input)
        {
            if (input.Ids == null || !input.Ids.Any())
            {
                return new ListResultDto<LessonSessionDto>(new List<LessonSessionDto>());
            }

            var entities = await Repository.GetAll()
                .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                .Include(x => x.Subject).ThenInclude(x => x.Name)
                .Include(x => x.Unit).ThenInclude(x => x.Name)
                .Include(x => x.TeacherProfile)
                .Where(x => input.Ids.Contains(x.Id))
                .ToListAsync();

            var dtos = new List<LessonSessionDto>();
            foreach (var entity in entities)
            {
                dtos.Add(await MapLessonSessionToDto(entity));
            }

            return new ListResultDto<LessonSessionDto>(dtos);
        }

        private async Task<LessonSessionDto> MapLessonSessionToDto(LessonSession entity)
        {
            var dto = MapToEntityDto(entity);
            dto.Attachments = new List<LiteAttachmentDto>();

            if (entity.Attachments != null)
            {
                foreach (var lessonAttachment in entity.Attachments)
                {
                    if (lessonAttachment.Attachment != null)
                    {
                        var attDto = ObjectMapper.Map<LiteAttachmentDto>(lessonAttachment.Attachment);
                        attDto.Url = _attachmentManager.GetUrl(lessonAttachment.Attachment);
                        dto.Attachments.Add(attDto);
                    }
                }
            }

            if (entity.ThumbnailAttachmentId.HasValue)
            {
                var thumbnail = await _attachmentRepository.FirstOrDefaultAsync(entity.ThumbnailAttachmentId.Value);
                if (thumbnail != null)
                {
                    dto.Thumbnail = ObjectMapper.Map<LiteAttachmentDto>(thumbnail);
                    dto.Thumbnail.Url = _attachmentManager.GetUrl(thumbnail);
                    dto.Thumbnail.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(thumbnail);
                }
            }

            if (entity.VideoAttachmentId.HasValue)
            {
                var video = await _attachmentRepository.FirstOrDefaultAsync(entity.VideoAttachmentId.Value);
                if (video != null)
                {
                    dto.Video = ObjectMapper.Map<LiteAttachmentDto>(video);
                    dto.Video.Url = _attachmentManager.GetUrl(video);
                }
            }

            // Map Teacher Details
            if (entity.TeacherProfile != null)
            {
                dto.TeacherName = entity.TeacherProfile.Name;
                dto.TeacherBio = entity.TeacherProfile.Bio;
                dto.TeacherSpecialization = entity.TeacherProfile.Specialization;
                dto.TeacherRating = entity.TeacherProfile.AverageRating;
                dto.TeacherReviewsCount = entity.TeacherProfile.ReviewsCount;

                if (entity.TeacherProfile.AttachmentId.HasValue)
                {
                    var teacherImg = await _attachmentRepository.FirstOrDefaultAsync(entity.TeacherProfile.AttachmentId.Value);
                    if (teacherImg != null)
                    {
                        dto.TeacherImageUrl = _attachmentManager.GetUrl(teacherImg);
                    }
                }
            }

            // Map Subject Name
            if (entity.Subject != null)
            {
                var currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                dto.SubjectName = entity.Subject.Name?.FirstOrDefault(x => x.Code == currentLanguage)?.Name
                    ?? entity.Subject.Name?.FirstOrDefault()?.Name;
            }

            // Map Unit Name
            if (entity.Unit != null)
            {
                var currentLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                dto.UnitName = entity.Unit.Name?.FirstOrDefault(x => x.Code == currentLanguage)?.Name
                    ?? entity.Unit.Name?.FirstOrDefault()?.Name;
            }

            // Check progress for current user
            var userId = AbpSession.UserId;
            if (userId.HasValue)
            {
                var progress = await _progressRepository.FirstOrDefaultAsync(p => p.SessionId == entity.Id && p.UserId == userId.Value);
                dto.IsCompleted = progress?.IsCompleted ?? false;
                dto.CanTakeQuiz = progress?.CanTakeQuiz ?? false;
                dto.WatchedSeconds = progress?.WatchedSeconds ?? 0;
                dto.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == entity.Id && x.ItemType == SavedItemType.Session);
            }

            return dto;
        }

        public async Task MarkAsCompleteAsync(EntityDto<Guid> input)
        {
            await UpdateProgressAsync(new UpdateLessonProgressInput { SessionId = input.Id, IsCompleted = true });
        }

        public async Task UpdateProgressAsync(UpdateLessonProgressInput input)
        {
            var userId = AbpSession.GetUserId();
            var progress = await _progressRepository.FirstOrDefaultAsync(p => p.SessionId == input.SessionId && p.UserId == userId);

            if (progress == null)
            {
                progress = new UserSessionProgress
                {
                    SessionId = input.SessionId,
                    UserId = userId,
                    WatchedSeconds = input.WatchedSeconds,
                    IsCompleted = input.IsCompleted,
                    CanTakeQuiz = input.IsCompleted,
                    LastWatchedAt = DateTime.Now
                };
                await _progressRepository.InsertAsync(progress);
            }
            else
            {
                if (input.WatchedSeconds > progress.WatchedSeconds)
                {
                    progress.WatchedSeconds = input.WatchedSeconds;
                }
                if (input.IsCompleted)
                {
                    progress.IsCompleted = true;
                    progress.CanTakeQuiz = true;
                }
                progress.LastWatchedAt = DateTime.Now;
                await _progressRepository.UpdateAsync(progress);
            }

            // check 90% logic
            var session = await Repository.FirstOrDefaultAsync(input.SessionId);
            if (session != null && session.DurationSeconds > 0)
            {
                if ((double)progress.WatchedSeconds / session.DurationSeconds >= 0.9)
                {
                    progress.CanTakeQuiz = true;
                }
            }

            // If it's a short session or finished, ensure it's unlocked
            if (input.IsCompleted)
            {
                 progress.CanTakeQuiz = true;
            }

            // Update user LastStudiedAt
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.LastStudiedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task ReportIssueAsync(ReportSessionIssueInput input)
        {
            await Task.CompletedTask;
        }
    }
}

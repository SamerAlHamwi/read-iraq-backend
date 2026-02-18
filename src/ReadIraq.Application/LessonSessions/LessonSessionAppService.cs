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
using static ReadIraq.Enums.Enum;
using System.Collections.Generic;

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

        public LessonSessionAppService(
            IRepository<LessonSession, Guid> repository,
            IRepository<UserSessionProgress, Guid> progressRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository)
            : base(repository)
        {
            _progressRepository = progressRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
        }

        protected override IQueryable<LessonSession> CreateFilteredQuery(PagedLessonSessionResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                    x => x.Title.Contains(input.Keyword) || x.Description.Contains(input.Keyword))
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId.Value)
                .WhereIf(input.IsFree.HasValue, x => x.IsFree == input.IsFree.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        public override async Task<PagedResultDto<LiteLessonSessionDto>> GetAllAsync(PagedLessonSessionResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);

            foreach (var item in result.Items)
            {
                var thumbnail = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.LessonSessionThumbnail);
                if (thumbnail != null)
                {
                    item.Thumbnail = ObjectMapper.Map<LiteAttachmentDto>(thumbnail);
                    item.Thumbnail.Url = _attachmentManager.GetUrl(thumbnail);
                    item.Thumbnail.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(thumbnail);
                }

                var video = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.LessonSessionVideo);
                if (video != null)
                {
                    item.Video = ObjectMapper.Map<LiteAttachmentDto>(video);
                    item.Video.Url = _attachmentManager.GetUrl(video);
                }
            }

            return result;
        }

        public override async Task<LessonSessionDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Attachments).ThenInclude(x => x.Attachment)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Session not found");
            }

            var dto = MapToEntityDto(entity);
            dto.Attachments = new List<LiteAttachmentDto>();

            foreach (var lessonAttachment in entity.Attachments)
            {
                var attDto = ObjectMapper.Map<LiteAttachmentDto>(lessonAttachment.Attachment);
                attDto.Url = _attachmentManager.GetUrl(lessonAttachment.Attachment);
                dto.Attachments.Add(attDto);
            }

            var thumbnail = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.LessonSessionThumbnail);
            if (thumbnail != null)
            {
                dto.Thumbnail = ObjectMapper.Map<LiteAttachmentDto>(thumbnail);
                dto.Thumbnail.Url = _attachmentManager.GetUrl(thumbnail);
                dto.Thumbnail.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(thumbnail);
            }

            var video = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.LessonSessionVideo);
            if (video != null)
            {
                dto.Video = ObjectMapper.Map<LiteAttachmentDto>(video);
                dto.Video.Url = _attachmentManager.GetUrl(video);
            }

            // Check progress for current user
            var userId = AbpSession.UserId;
            if (userId.HasValue)
            {
                var progress = await _progressRepository.FirstOrDefaultAsync(p => p.SessionId == input.Id && p.UserId == userId.Value);
                dto.IsCompleted = progress?.IsCompleted ?? false;
                dto.WatchedSeconds = progress?.WatchedSeconds ?? 0;
            }

            return dto;
        }

        public override async Task<LessonSessionDto> CreateAsync(CreateLessonSessionDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.ViewsCount = 0;
            entity.LikesCount = 0;
            entity.IsActive = true;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.ThumbnailAttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.ThumbnailAttachmentId, AttachmentRefType.LessonSessionThumbnail, entity.Id.ToString());
            }

            if (input.VideoAttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.VideoAttachmentId, AttachmentRefType.LessonSessionVideo, entity.Id.ToString());
            }

            if (input.AttachmentIds != null)
            {
                foreach (var attachmentId in input.AttachmentIds.Distinct())
                {
                    entity.Attachments.Add(new LessonSessionAttachment { AttachmentId = attachmentId });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public override async Task<LessonSessionDto> UpdateAsync(UpdateLessonSessionDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Session not found");
            }

            MapToEntity(input, entity);

            if (input.ThumbnailAttachmentId > 0)
            {
                var oldThumbnail = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.LessonSessionThumbnail);
                if (oldThumbnail != null && oldThumbnail.Id != input.ThumbnailAttachmentId)
                {
                    await _attachmentManager.DeleteRefIdAsync(oldThumbnail);
                }
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.ThumbnailAttachmentId, AttachmentRefType.LessonSessionThumbnail, entity.Id.ToString());
            }

            if (input.VideoAttachmentId > 0)
            {
                var oldVideo = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.LessonSessionVideo);
                if (oldVideo != null && oldVideo.Id != input.VideoAttachmentId)
                {
                    await _attachmentManager.DeleteRefIdAsync(oldVideo);
                }
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.VideoAttachmentId, AttachmentRefType.LessonSessionVideo, entity.Id.ToString());
            }

            entity.Attachments.Clear();
            if (input.AttachmentIds != null)
            {
                foreach (var attachmentId in input.AttachmentIds.Distinct())
                {
                    entity.Attachments.Add(new LessonSessionAttachment
                    {
                        LessonSessionId = entity.Id,
                        AttachmentId = attachmentId
                    });
                }
            }

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public async Task MarkAsCompleteAsync(EntityDto<Guid> input)
        {
            var userId = AbpSession.GetUserId();
            var progress = await _progressRepository.FirstOrDefaultAsync(p => p.SessionId == input.Id && p.UserId == userId);

            if (progress == null)
            {
                await _progressRepository.InsertAsync(new UserSessionProgress
                {
                    SessionId = input.Id,
                    UserId = userId,
                    IsCompleted = true,
                    LastWatchedAt = DateTime.Now
                });
            }
            else
            {
                progress.IsCompleted = true;
                progress.LastWatchedAt = DateTime.Now;
                await _progressRepository.UpdateAsync(progress);
            }
        }

        public async Task ReportIssueAsync(ReportSessionIssueInput input)
        {
            await Task.CompletedTask;
        }
    }
}

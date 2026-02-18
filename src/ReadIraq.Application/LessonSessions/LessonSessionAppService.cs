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

namespace ReadIraq.LessonSessions
{
    [AbpAuthorize]
    public class LessonSessionAppService : ReadIraqAsyncCrudAppService<LessonSession, LessonSessionDto, Guid, LiteLessonSessionDto,
        PagedLessonSessionResultRequestDto, CreateLessonSessionDto, UpdateLessonSessionDto>,
        ILessonSessionAppService
    {
        private readonly IRepository<UserSessionProgress, Guid> _progressRepository;

        public LessonSessionAppService(
            IRepository<LessonSession, Guid> repository,
            IRepository<UserSessionProgress, Guid> progressRepository)
            : base(repository)
        {
            _progressRepository = progressRepository;
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

        public override async Task<LessonSessionDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Session not found");
            }

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();

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

            if (input.AttachmentIds != null)
            {
                foreach (var attachmentId in input.AttachmentIds.Distinct())
                {
                    entity.Attachments.Add(new LessonSessionAttachment { AttachmentId = attachmentId });
                }
            }

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();
            return dto;
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

            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.AttachmentIds = entity.Attachments.Select(a => a.AttachmentId).ToList();
            return dto;
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
            // Logic to record report (e.g. in a Reports table or send email)
            // For now, just a placeholder.
            await Task.CompletedTask;
        }
    }
}

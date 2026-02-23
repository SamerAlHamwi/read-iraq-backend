using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Teachers;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Follows;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherProfileAppService : ReadIraqAsyncCrudAppService<TeacherProfile, TeacherProfileDto, Guid, LiteTeacherProfileDto, PagedTeacherProfileResultRequestDto, CreateTeacherProfileDto, UpdateTeacherProfileDto>, ITeacherProfileAppService
    {
        private readonly ITeacherProfileManager _teacherProfileManager;
        private readonly IRepository<TeacherFeatureMap, Guid> _featureMapRepository;
        private readonly IRepository<TeacherSubject, Guid> _teacherSubjectRepository;
        private readonly IRepository<LessonSession, Guid> _lessonSessionRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<TeacherFeature, Guid> _featureRepository;
        private readonly IRepository<UserFollowTeacher, Guid> _userFollowTeacherRepository;

        public TeacherProfileAppService(
            IRepository<TeacherProfile, Guid> repository,
            ITeacherProfileManager teacherProfileManager,
            IRepository<TeacherFeatureMap, Guid> featureMapRepository,
            IRepository<TeacherSubject, Guid> teacherSubjectRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IAttachmentManager attachmentManager,
            IRepository<TeacherFeature, Guid> featureRepository,
            IRepository<UserFollowTeacher, Guid> userFollowTeacherRepository)
            : base(repository)
        {
            _teacherProfileManager = teacherProfileManager;
            _featureMapRepository = featureMapRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _attachmentManager = attachmentManager;
            _featureRepository = featureRepository;
            _userFollowTeacherRepository = userFollowTeacherRepository;
        }

        protected override IQueryable<TeacherProfile> CreateFilteredQuery(PagedTeacherProfileResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Specialization.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);

            if (input.SubjectId.HasValue)
            {
                query = query.Where(x => x.Subjects.Any(s => s.SubjectId == input.SubjectId.Value));
            }

            return query;
        }

        public override async Task<PagedResultDto<LiteTeacherProfileDto>> GetAllAsync(PagedTeacherProfileResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            var userId = AbpSession.UserId;

            foreach (var item in result.Items)
            {
                var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.TeacherProfile);
                if (attachment != null)
                {
                    item.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                    item.Attachment.Url = _attachmentManager.GetUrl(attachment);
                    item.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                }

                item.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.TeacherProfileId == item.Id && x.IsActive);

                if (userId.HasValue)
                {
                    item.IsFollowed = await _userFollowTeacherRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.TeacherProfileId == item.Id);
                }
            }

            return result;
        }

        public override async Task<TeacherProfileDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .Include(x => x.RatingBreakdowns)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Teacher not found");
            }

            var dto = MapToEntityDto(entity);
            dto.FeatureIds = entity.Features.Select(f => f.TeacherFeatureId).ToList();
            dto.SubjectIds = entity.Subjects.Select(s => s.SubjectId).ToList();

            var features = await _featureRepository.GetAllListAsync(x => dto.FeatureIds.Contains(x.Id));
            dto.Features = ObjectMapper.Map<List<TeacherFeatureDto>>(features);

            dto.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.TeacherProfileId == entity.Id && x.IsActive);

            var attachment = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.TeacherProfile);
            if (attachment != null)
            {
                dto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                dto.Attachment.Url = _attachmentManager.GetUrl(attachment);
                dto.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
            }

            var userId = AbpSession.UserId;
            if (userId.HasValue)
            {
                dto.IsFollowed = await _userFollowTeacherRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.TeacherProfileId == entity.Id);
            }

            return dto;
        }

        public override async Task<TeacherProfileDto> CreateAsync(CreateTeacherProfileDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.IsActive = true;

            if (input.FeatureIds != null)
            {
                foreach (var featureId in input.FeatureIds)
                {
                    entity.Features.Add(new TeacherFeatureMap { TeacherFeatureId = featureId });
                }
            }

            if (input.SubjectIds != null)
            {
                foreach (var subjectId in input.SubjectIds)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = subjectId });
                }
            }

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.AttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.TeacherProfile, entity.Id.ToString());
            }

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public override async Task<TeacherProfileDto> UpdateAsync(UpdateTeacherProfileDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Teacher not found");
            }

            MapToEntity(input, entity);

            if (input.AttachmentId > 0)
            {
                var oldAttachment = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.TeacherProfile);
                if (oldAttachment != null && oldAttachment.Id != input.AttachmentId)
                {
                    await _attachmentManager.DeleteRefIdAsync(oldAttachment);
                }
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.TeacherProfile, entity.Id.ToString());
            }

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public async Task AssignSubjectsAsync(AssignSubjectsInput input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.Id == input.TeacherProfileId);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Teacher not found");
            }

            entity.Subjects.Clear();
            if (input.SubjectIds != null)
            {
                foreach (var subjectId in input.SubjectIds)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = subjectId, TeacherProfileId = entity.Id });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<TeacherStatsDto> GetStatsAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);

            return new TeacherStatsDto
            {
                StudentsCount = entity.StudentsCount,
                WatchTimeMinutes = 0,
                QuizAttemptsCount = 0,
                AverageQuizScore = 0
            };
        }

        public async Task ToggleActiveAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = !entity.IsActive;
            await Repository.UpdateAsync(entity);
        }

        [HttpPost]
        public async Task FollowAsync(EntityDto<Guid> input)
        {
            var userId = AbpSession.GetUserId();
            var exists = await _userFollowTeacherRepository.GetAll().AnyAsync(x => x.UserId == userId && x.TeacherProfileId == input.Id);
            if (!exists)
            {
                await _userFollowTeacherRepository.InsertAsync(new UserFollowTeacher
                {
                    UserId = userId,
                    TeacherProfileId = input.Id
                });
            }
        }

        [HttpPost]
        public async Task UnfollowAsync(EntityDto<Guid> input)
        {
            var userId = AbpSession.GetUserId();
            var follow = await _userFollowTeacherRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.TeacherProfileId == input.Id);
            if (follow != null)
            {
                await _userFollowTeacherRepository.DeleteAsync(follow.Id);
            }
        }
    }
}

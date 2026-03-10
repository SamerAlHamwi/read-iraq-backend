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
using ReadIraq.Domain.SavedItems;
using static ReadIraq.Enums.Enum;
using Abp.UI;
using Abp.Domain.Uow;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Authorization.Users;

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
        private readonly IRepository<Attachment, long> _attachmentRepository;
        private readonly IRepository<TeacherFeature, Guid> _featureRepository;
        private readonly IRepository<UserFollowTeacher, Guid> _userFollowTeacherRepository;
        private readonly IRepository<UserSavedItem, Guid> _userSavedItemRepository;
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<UserPreferredSubject, Guid> _userPreferredSubjectRepository;
        private readonly IRepository<UserPreferredTeacher, Guid> _userPreferredTeacherRepository;
        private readonly IRepository<GradeSubject> _gradeSubjectRepository;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Grade, int> _gradeRepository;

        public TeacherProfileAppService(
            IRepository<TeacherProfile, Guid> repository,
            ITeacherProfileManager teacherProfileManager,
            IRepository<TeacherFeatureMap, Guid> featureMapRepository,
            IRepository<TeacherSubject, Guid> teacherSubjectRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository,
            IRepository<TeacherFeature, Guid> featureRepository,
            IRepository<UserFollowTeacher, Guid> userFollowTeacherRepository,
            IRepository<UserSavedItem, Guid> userSavedItemRepository,
            IRepository<Subject, Guid> subjectRepository,
            IRepository<UserPreferredSubject, Guid> userPreferredSubjectRepository,
            IRepository<UserPreferredTeacher, Guid> userPreferredTeacherRepository,
            IRepository<GradeSubject> gradeSubjectRepository,
            IRepository<Enrollment, Guid> enrollmentRepository,
            IRepository<User, long> userRepository,
            IRepository<Grade, int> gradeRepository)
            : base(repository)
        {
            _teacherProfileManager = teacherProfileManager;
            _featureMapRepository = featureMapRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _featureRepository = featureRepository;
            _userFollowTeacherRepository = userFollowTeacherRepository;
            _userSavedItemRepository = userSavedItemRepository;
            _subjectRepository = subjectRepository;
            _userPreferredSubjectRepository = userPreferredSubjectRepository;
            _userPreferredTeacherRepository = userPreferredTeacherRepository;
            _gradeSubjectRepository = gradeSubjectRepository;
            _enrollmentRepository = enrollmentRepository;
            _userRepository = userRepository;
            _gradeRepository = gradeRepository;
        }

        protected override IQueryable<TeacherProfile> CreateFilteredQuery(PagedTeacherProfileResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Specialization.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);

            if (input.SubjectId.HasValue && input.GradeId.HasValue)
            {
                query = query.Where(x => x.Subjects.Any(s => s.SubjectId == input.SubjectId.Value && s.GradeId == input.GradeId.Value));
            }
            else if (input.SubjectId.HasValue)
            {
                query = query.Where(x => x.Subjects.Any(s => s.SubjectId == input.SubjectId.Value));
            }
            else if (input.GradeId.HasValue)
            {
                query = query.Where(x => x.Subjects.Any(s => s.GradeId == input.GradeId.Value));
            }

            return query;
        }

        private async Task ValidateTeacherProfileInput(long userId, long attachmentId, List<Guid> featureIds, List<TeacherSubjectDto> subjects)
        {
            if (userId > 0 && !await _userRepository.GetAll().AnyAsync(x => x.Id == userId))
            {
                throw new Abp.UI.UserFriendlyException(L("UserNotFound"));
            }

            if (attachmentId > 0 && !await _attachmentRepository.GetAll().AnyAsync(x => x.Id == attachmentId))
            {
                throw new Abp.UI.UserFriendlyException(L("AttachmentNotFound"));
            }

            if (featureIds != null && featureIds.Any())
            {
                foreach (var featureId in featureIds)
                {
                    if (!await _featureRepository.GetAll().AnyAsync(x => x.Id == featureId))
                    {
                        throw new Abp.UI.UserFriendlyException(L("TeacherFeatureNotFound") + ": " + featureId);
                    }
                }
            }

            if (subjects != null && subjects.Any())
            {
                foreach (var s in subjects)
                {
                    if (!await _subjectRepository.GetAll().AnyAsync(x => x.Id == s.SubjectId))
                    {
                        throw new Abp.UI.UserFriendlyException(L("SubjectNotFound") + ": " + s.SubjectId);
                    }

                    if (!await _gradeRepository.GetAll().AnyAsync(x => x.Id == s.GradeId))
                    {
                        throw new Abp.UI.UserFriendlyException(L("GradeNotFound") + ": " + s.GradeId);
                    }
                }
            }
        }

        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteTeacherProfileDto>> GetAllAsync(PagedTeacherProfileResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            var userId = AbpSession.UserId;

            foreach (var item in result.Items)
            {
                var entity = await Repository.FirstOrDefaultAsync(item.Id);
                if (entity != null && entity.AttachmentId.HasValue)
                {
                    var attachment = await _attachmentRepository.FirstOrDefaultAsync(entity.AttachmentId.Value);
                    if (attachment != null)
                    {
                        item.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                        item.Attachment.Url = _attachmentManager.GetUrl(attachment);
                        item.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                    }
                }

                item.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.TeacherProfileId == item.Id && x.IsActive);

                // Count students enrolled in any subject related to this teacher
                item.StudentsCount = await _enrollmentRepository.GetAll()
                    .Where(e => _teacherSubjectRepository.GetAll()
                        .Any(ts => ts.TeacherProfileId == item.Id && ts.SubjectId == e.SubjectId && ts.GradeId == e.GradeId))
                    .Select(e => e.UserId)
                    .Distinct()
                    .CountAsync();

                if (userId.HasValue)
                {
                    item.IsFollowed = await _userFollowTeacherRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.TeacherProfileId == item.Id);
                    item.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == item.Id && x.ItemType == SavedItemType.Teacher);
                }
            }

            return result;
        }

        [AbpAllowAnonymous]
        public override async Task<TeacherProfileDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .Include(x => x.RatingBreakdowns)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("TeacherNotFound"));
            }

            var dto = MapToEntityDto(entity);
            dto.FeatureIds = entity.Features.Select(f => f.TeacherFeatureId).ToList();
            dto.Subjects = entity.Subjects.Select(s => new TeacherSubjectDto { SubjectId = s.SubjectId, GradeId = s.GradeId }).ToList();

            var features = await _featureRepository.GetAllListAsync(x => dto.FeatureIds.Contains(x.Id));
            dto.Features = ObjectMapper.Map<List<TeacherFeatureDto>>(features);

            dto.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.TeacherProfileId == entity.Id && x.IsActive);

            // Count students enrolled in any subject related to this teacher
            dto.StudentsCount = await _enrollmentRepository.GetAll()
                .Where(e => _teacherSubjectRepository.GetAll()
                    .Any(ts => ts.TeacherProfileId == entity.Id && ts.SubjectId == e.SubjectId && ts.GradeId == e.GradeId))
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            if (entity.AttachmentId.HasValue)
            {
                var attachment = await _attachmentRepository.FirstOrDefaultAsync(entity.AttachmentId.Value);
                if (attachment != null)
                {
                    dto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                    dto.Attachment.Url = _attachmentManager.GetUrl(attachment);
                    dto.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                }
            }

            var userId = AbpSession.UserId;
            if (userId.HasValue)
            {
                dto.IsFollowed = await _userFollowTeacherRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.TeacherProfileId == entity.Id);
                dto.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == entity.Id && x.ItemType == SavedItemType.Teacher);

                // Check if user is enrolled in any subject taught by this teacher
                dto.IsEnrolled = await _enrollmentRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && _teacherSubjectRepository.GetAll()
                    .Any(ts => ts.TeacherProfileId == entity.Id && ts.SubjectId == x.SubjectId && ts.GradeId == x.GradeId));
            }

            return dto;
        }

        protected override async Task<TeacherProfile> GetEntityByIdAsync(Guid id)
        {
            var entity = await Repository.FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("TeacherNotFound"));
            }
            return entity;
        }

        public override async Task<TeacherProfileDto> CreateAsync(CreateTeacherProfileDto input)
        {
            CheckCreatePermission();

            await ValidateTeacherProfileInput(input.UserId, input.AttachmentId, input.FeatureIds, input.Subjects);

            var entity = MapToEntity(input);
            entity.IsActive = true;

            if (input.FeatureIds != null)
            {
                foreach (var featureId in input.FeatureIds)
                {
                    entity.Features.Add(new TeacherFeatureMap { TeacherFeatureId = featureId });
                }
            }

            if (input.Subjects != null)
            {
                foreach (var s in input.Subjects)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = s.SubjectId, GradeId = s.GradeId });
                }
            }

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.AttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.TeacherProfile, entity.Id.ToString());
                entity.AttachmentId = input.AttachmentId;
            }

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public override async Task<TeacherProfileDto> UpdateAsync(UpdateTeacherProfileDto input)
        {
            CheckUpdatePermission();

            await ValidateTeacherProfileInput(input.UserId, input.AttachmentId, input.FeatureIds, input.Subjects);

            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("TeacherNotFound"));
            }

            MapToEntity(input, entity);

            if (input.FeatureIds != null)
            {
                entity.Features.Clear();
                foreach (var featureId in input.FeatureIds)
                {
                    entity.Features.Add(new TeacherFeatureMap { TeacherFeatureId = featureId });
                }
            }

            if (input.Subjects != null)
            {
                entity.Subjects.Clear();
                foreach (var s in input.Subjects)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = s.SubjectId, GradeId = s.GradeId });
                }
            }

            if (input.AttachmentId > 0)
            {
                var oldAttachment = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.TeacherProfile);
                if (oldAttachment != null && oldAttachment.Id != input.AttachmentId)
                {
                    await _attachmentManager.DeleteRefIdAsync(oldAttachment);
                }
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.TeacherProfile, entity.Id.ToString());
                entity.AttachmentId = input.AttachmentId;
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
                throw new Abp.UI.UserFriendlyException(L("TeacherNotFound"));
            }

            if (input.Subjects != null && input.Subjects.Any())
            {
                foreach (var s in input.Subjects)
                {
                    if (!await _subjectRepository.GetAll().AnyAsync(x => x.Id == s.SubjectId))
                    {
                        throw new Abp.UI.UserFriendlyException(L("SubjectNotFound") + ": " + s.SubjectId);
                    }

                    if (!await _gradeRepository.GetAll().AnyAsync(x => x.Id == s.GradeId))
                    {
                        throw new Abp.UI.UserFriendlyException(L("GradeNotFound") + ": " + s.GradeId);
                    }
                }
            }

            entity.Subjects.Clear();
            if (input.Subjects != null)
            {
                foreach (var s in input.Subjects)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = s.SubjectId, GradeId = s.GradeId, TeacherProfileId = entity.Id });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<TeacherStatsDto> GetStatsAsync(EntityDto<Guid> input)
        {
            var studentsCount = await _enrollmentRepository.GetAll()
                .Where(e => _teacherSubjectRepository.GetAll()
                    .Any(ts => ts.TeacherProfileId == input.Id && ts.SubjectId == e.SubjectId && ts.GradeId == e.GradeId))
                .Select(e => e.UserId)
                .Distinct()
                .CountAsync();

            return new TeacherStatsDto
            {
                StudentsCount = studentsCount,
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
            if (input == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("Teacher Id is required");
            }

            var userId = AbpSession.GetUserId();
            var teacherProfileId = input.Id;

            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var existingFollow = await _userFollowTeacherRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.TeacherProfileId == teacherProfileId);
                if (existingFollow != null)
                {
                    if (existingFollow.IsDeleted)
                    {
                        existingFollow.IsDeleted = false;
                        existingFollow.DeletionTime = null;
                        existingFollow.DeleterUserId = null;
                        await _userFollowTeacherRepository.UpdateAsync(existingFollow);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                    return;
                }
            }

            // Verify teacher exists
            if (!await Repository.GetAll().AnyAsync(x => x.Id == teacherProfileId))
            {
                throw new UserFriendlyException(L("TeacherNotFound"));
            }

            await _userFollowTeacherRepository.InsertAsync(new UserFollowTeacher
            {
                UserId = userId,
                TeacherProfileId = teacherProfileId
            });

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        [HttpPost]
        public async Task UnfollowAsync(EntityDto<Guid> input)
        {
            if (input == null || input.Id == Guid.Empty)
            {
                throw new UserFriendlyException("Teacher Id is required");
            }

            var userId = AbpSession.GetUserId();
            var teacherProfileId = input.Id;

            var follow = await _userFollowTeacherRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.TeacherProfileId == teacherProfileId);
            if (follow != null)
            {
                await _userFollowTeacherRepository.DeleteAsync(follow.Id);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<List<PreferredTeachersSectionDto>> GetPreferredTeachersByGradeAsync(int gradeId)
        {
            var userId = AbpSession.UserId;
            var currentLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;

            // 1. Get all subjects for this grade
            var subjects = await _subjectRepository.GetAll()
                .Include(x => x.Name)
                .Where(s => _gradeSubjectRepository.GetAll().Any(gs => gs.SubjectId == s.Id && gs.GradeId == gradeId))
                .ToListAsync();

            var preferredSubjectIds = new List<Guid>();
            var preferredTeacherMaps = new List<UserPreferredTeacher>();

            if (userId.HasValue)
            {
                preferredSubjectIds = await _userPreferredSubjectRepository.GetAll()
                    .Where(x => x.UserId == userId.Value)
                    .Select(x => x.SubjectId)
                    .ToListAsync();

                preferredTeacherMaps = await _userPreferredTeacherRepository.GetAll()
                    .Where(x => x.UserId == userId.Value)
                    .ToListAsync();
            }

            // 2. Order subjects: Preferred first, then others
            var orderedSubjects = subjects
                .OrderByDescending(s => preferredSubjectIds.Contains(s.Id))
                .ToList();

            var result = new List<PreferredTeachersSectionDto>();

            foreach (var subject in orderedSubjects)
            {
                // Get all teachers for this subject and grade
                var teachers = await _teacherSubjectRepository.GetAll()
                    .Include(ts => ts.TeacherProfile)
                    .Where(ts => ts.SubjectId == subject.Id && ts.GradeId == gradeId && ts.TeacherProfile.IsActive)
                    .Select(ts => ts.TeacherProfile)
                    .ToListAsync();

                // Order teachers: Preferred first, then by rating
                var subjectPreferredTeacherIds = preferredTeacherMaps
                    .Where(x => x.SubjectId == subject.Id)
                    .Select(x => x.TeacherProfileId)
                    .ToList();

                var orderedTeachers = teachers
                    .OrderByDescending(t => subjectPreferredTeacherIds.Contains(t.Id))
                    .ThenByDescending(t => t.AverageRating)
                    .ToList();

                var teacherDtos = ObjectMapper.Map<List<LiteTeacherProfileDto>>(orderedTeachers);

                // Attachments
                foreach (var teacherDto in teacherDtos)
                {
                    var profile = teachers.First(x => x.Id == teacherDto.Id);
                    if (profile.AttachmentId.HasValue)
                    {
                        var attachment = await _attachmentRepository.FirstOrDefaultAsync(profile.AttachmentId.Value);
                        if (attachment != null)
                        {
                            teacherDto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                            teacherDto.Attachment.Url = _attachmentManager.GetUrl(attachment);
                            teacherDto.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                        }
                    }

                    // Count students enrolled in any subject related to this teacher
                    teacherDto.StudentsCount = await _enrollmentRepository.GetAll()
                        .Where(e => _teacherSubjectRepository.GetAll()
                            .Any(ts => ts.TeacherProfileId == teacherDto.Id && ts.SubjectId == e.SubjectId && ts.GradeId == e.GradeId))
                        .Select(e => e.UserId)
                        .Distinct()
                        .CountAsync();
                }

                var localizedTitle = subject.Name.FirstOrDefault(x => x.Code == currentLanguage)?.Name
                                     ?? subject.Name.FirstOrDefault()?.Name
                                     ?? subject.Id.ToString();

                result.Add(new PreferredTeachersSectionDto
                {
                    SubjectId = subject.Id,
                    Title = localizedTitle,
                    Teachers = teacherDtos
                });
            }

            return result;
        }
    }
}

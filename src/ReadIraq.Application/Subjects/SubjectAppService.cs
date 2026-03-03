using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Units;
using ReadIraq.Subjects.Dto;
using ReadIraq.Units.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.SavedItems;
using ReadIraq.Teachers.Dto;
using ReadIraq.LessonSessions.Dto;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Enrollments;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Subjects
{
    [AbpAuthorize]
    public class SubjectAppService : ReadIraqAsyncCrudAppService<Subject, SubjectDto, Guid, LiteSubjectDto, PagedSubjectResultRequestDto, CreateSubjectDto, UpdateSubjectDto>, ISubjectAppService
    {
        private readonly ISubjectManager _subjectManager;
        private readonly IRepository<GradeSubject> _gradeSubjectRepository;
        private readonly IRepository<TeacherSubject, Guid> _teacherSubjectRepository;
        private readonly IRepository<LessonSession, Guid> _lessonSessionRepository;
        private readonly IRepository<UserPreferredTeacher, Guid> _userPreferredTeacherRepository;
        private readonly IRepository<UserPreferredSubject, Guid> _userPreferredSubjectRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Attachment, long> _attachmentRepository;
        private readonly IRepository<UserSavedItem, Guid> _userSavedItemRepository;
        private readonly IRepository<Unit, Guid> _unitRepository;
        private readonly IRepository<Enrollment, Guid> _enrollmentRepository;

        public SubjectAppService(
            IRepository<Subject, Guid> repository,
            ISubjectManager subjectManager,
            IRepository<GradeSubject> gradeSubjectRepository,
            IRepository<TeacherSubject, Guid> teacherSubjectRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IRepository<UserPreferredTeacher, Guid> userPreferredTeacherRepository,
            IRepository<UserPreferredSubject, Guid> userPreferredSubjectRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository,
            IRepository<UserSavedItem, Guid> userSavedItemRepository,
            IRepository<Unit, Guid> unitRepository,
            IRepository<Enrollment, Guid> enrollmentRepository)
            : base(repository)
        {
            _subjectManager = subjectManager;
            _gradeSubjectRepository = gradeSubjectRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _userPreferredTeacherRepository = userPreferredTeacherRepository;
            _userPreferredSubjectRepository = userPreferredSubjectRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _userSavedItemRepository = userSavedItemRepository;
            _unitRepository = unitRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        protected override IQueryable<Subject> CreateFilteredQuery(PagedSubjectResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input)
                .Include(x => x.Name)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Any(t => t.Name.Contains(input.Keyword)) || x.Description.Contains(input.Keyword))
                .WhereIf(input.Level.HasValue, x => x.Level == input.Level.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);

            if (input.GradeId.HasValue && !input.TeacherProfileId.HasValue)
            {
                var subjectIds = _gradeSubjectRepository.GetAll()
                    .Where(gs => gs.GradeId == input.GradeId.Value)
                    .Select(gs => gs.SubjectId);
                query = query.Where(x => subjectIds.Contains(x.Id));
            }

            if (input.TeacherProfileId.HasValue)
            {
                var subjectIds = _teacherSubjectRepository.GetAll()
                    .Where(ts => ts.TeacherProfileId == input.TeacherProfileId.Value)
                    .WhereIf(input.GradeId.HasValue, ts => ts.GradeId == input.GradeId.Value)
                    .Select(ts => ts.SubjectId);
                query = query.Where(x => subjectIds.Contains(x.Id));
            }

            return query;
        }

        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteSubjectDto>> GetAllAsync(PagedSubjectResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            var userId = AbpSession.UserId;

            foreach (var item in result.Items)
            {
                var entity = await Repository.FirstOrDefaultAsync(item.Id);
                if (entity != null)
                {
                    item.StudentsCount = entity.StudentsCount;

                    if (entity.AttachmentId.HasValue)
                    {
                        var attachment = await _attachmentRepository.FirstOrDefaultAsync(entity.AttachmentId.Value);
                        if (attachment != null)
                        {
                            item.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                            item.Attachment.Url = _attachmentManager.GetUrl(attachment);
                            item.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                        }
                    }
                }

                item.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.SubjectId == item.Id);
                
                if (input.GradeId.HasValue)
                {
                    item.TeachersCount = await _teacherSubjectRepository.CountAsync(x => x.SubjectId == item.Id && x.GradeId == input.GradeId.Value && x.TeacherProfile.IsActive);
                }
                else
                {
                    item.TeachersCount = await _teacherSubjectRepository.CountAsync(x => x.SubjectId == item.Id && x.TeacherProfile.IsActive);
                }

                if (userId.HasValue)
                {
                    item.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == item.Id && x.ItemType == SavedItemType.Subject);
                }
            }

            return result;
        }

        [AbpAllowAnonymous]
        public override async Task<SubjectDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("SubjectNotFound"));
            }

            var dto = MapToEntityDto(entity);
            
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

            dto.LessonsCount = await _lessonSessionRepository.CountAsync(x => x.SubjectId == entity.Id);
            dto.TeachersCount = await _teacherSubjectRepository.CountAsync(x => x.SubjectId == entity.Id && x.TeacherProfile.IsActive);

            // Units and Lessons
            var units = await _unitRepository.GetAll()
                .Include(x => x.Name)
                .Where(x => x.SubjectId == entity.Id)
                .OrderBy(x => x.Order)
                .ToListAsync();

            dto.Units = ObjectMapper.Map<List<UnitDto>>(units);

            foreach (var unitDto in dto.Units)
            {
                var unitLessons = await _lessonSessionRepository.GetAllListAsync(x => x.UnitId == unitDto.Id);
                unitDto.Lessons = ObjectMapper.Map<List<LiteLessonSessionDto>>(unitLessons.OrderBy(x => x.Order));
            }

            // Progress
            var userId = AbpSession.UserId;
            if (userId.HasValue)
            {
                var preferredSubject = await _userPreferredSubjectRepository.FirstOrDefaultAsync(x => x.UserId == userId.Value && x.SubjectId == entity.Id);
                if (preferredSubject != null)
                {
                    dto.ProgressPercentage = (double)preferredSubject.ProgressPercent;
                }
                dto.IsSaved = await _userSavedItemRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.ItemId == entity.Id && x.ItemType == SavedItemType.Subject);
                dto.IsEnrolled = await _enrollmentRepository.GetAll().AnyAsync(x => x.UserId == userId.Value && x.SubjectId == entity.Id);
            }

            // Top Teacher
            var topTeacher = await _teacherSubjectRepository.GetAll()
                .Include(ts => ts.TeacherProfile)
                .Where(ts => ts.SubjectId == entity.Id && ts.TeacherProfile.IsActive)
                .OrderByDescending(ts => ts.TeacherProfile.AverageRating)
                .Select(ts => ts.TeacherProfile)
                .FirstOrDefaultAsync();

            if (topTeacher != null)
            {
                dto.TopTeacher = ObjectMapper.Map<LiteTeacherProfileDto>(topTeacher);
                if (topTeacher.AttachmentId.HasValue)
                {
                    var teacherImg = await _attachmentRepository.FirstOrDefaultAsync(topTeacher.AttachmentId.Value);
                    if (teacherImg != null)
                    {
                        dto.TopTeacher.Attachment = ObjectMapper.Map<LiteAttachmentDto>(teacherImg);
                        dto.TopTeacher.Attachment.Url = _attachmentManager.GetUrl(teacherImg);
                    }
                }
            }

            return dto;
        }

        public override async Task<SubjectDto> CreateAsync(CreateSubjectDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.IsActive = true;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.GradeIds != null && input.GradeIds.Any())
            {
                foreach (var gradeId in input.GradeIds)
                {
                    await _gradeSubjectRepository.InsertAsync(new GradeSubject { SubjectId = entity.Id, GradeId = gradeId });
                }
            }

            if (input.AttachmentId > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.Subject, entity.Id.ToString());
                entity.AttachmentId = input.AttachmentId;
            }

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public override async Task<SubjectDto> UpdateAsync(UpdateSubjectDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            MapToEntity(input, entity);

            if (input.GradeIds != null)
            {
                var existingGrades = await _gradeSubjectRepository.GetAllListAsync(gs => gs.SubjectId == entity.Id);
                var existingGradeIds = existingGrades.Select(eg => eg.GradeId).ToList();

                // Remove old
                foreach (var eg in existingGrades.Where(eg => !input.GradeIds.Contains(eg.GradeId)))
                {
                    await _gradeSubjectRepository.DeleteAsync(eg);
                }

                // Add new
                foreach (var gradeId in input.GradeIds.Where(gid => !existingGradeIds.Contains(gid)))
                {
                    await _gradeSubjectRepository.InsertAsync(new GradeSubject { SubjectId = entity.Id, GradeId = gradeId });
                }
            }

            if (input.AttachmentId > 0)
            {
                var oldAttachment = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.Subject);
                if (oldAttachment != null && oldAttachment.Id != input.AttachmentId)
                {
                    await _attachmentManager.DeleteRefIdAsync(oldAttachment);
                }
                await _attachmentManager.CheckAndUpdateRefIdAsync(input.AttachmentId, AttachmentRefType.Subject, entity.Id.ToString());
                entity.AttachmentId = input.AttachmentId;
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return await GetAsync(new EntityDto<Guid>(entity.Id));
        }

        public async Task ToggleActiveAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = !entity.IsActive;
            await Repository.UpdateAsync(entity);
        }

        public async Task<List<SubjectTeachersDto>> GetTeachersBySubjectsAsync(List<Guid> subjectIds, int? gradeId)
        {
            var subjects = await Repository.GetAll()
                .Include(x => x.Name)
                .Where(x => subjectIds.Contains(x.Id))
                .ToListAsync();

            var result = new List<SubjectTeachersDto>();

            foreach (var subject in subjects)
            {
                var teachers = await _teacherSubjectRepository.GetAll()
                    .Include(ts => ts.TeacherProfile)
                    .Where(ts => ts.SubjectId == subject.Id && ts.TeacherProfile.IsActive)
                    .WhereIf(gradeId.HasValue, ts => ts.GradeId == gradeId.Value)
                    .Select(ts => ts.TeacherProfile)
                    .ToListAsync();

                var teacherDtos = ObjectMapper.Map<List<LiteTeacherProfileDto>>(teachers);

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
                }

                result.Add(new SubjectTeachersDto
                {
                    SubjectId = subject.Id,
                    Name = ObjectMapper.Map<List<ReadIraq.Domain.Translations.Dto.TranslationDto>>(subject.Name),
                    Teachers = teacherDtos
                });
            }

            return result;
        }

        public async Task<List<LiteTeacherProfileDto>> GetTeachersBySubjectWithPriorityAsync(Guid subjectId)
        {
            var userId = AbpSession.GetUserId();

            var preferredTeacherIds = await _userPreferredTeacherRepository.GetAll()
                .Where(x => x.UserId == userId && x.SubjectId == subjectId)
                .Select(x => x.TeacherProfileId)
                .ToListAsync();

            var allTeachers = await _teacherSubjectRepository.GetAll()
                .Include(ts => ts.TeacherProfile)
                .Where(ts => ts.SubjectId == subjectId && ts.TeacherProfile.IsActive)
                .Select(ts => ts.TeacherProfile)
                .ToListAsync();

            var prioritizedTeachers = allTeachers
                .OrderByDescending(t => preferredTeacherIds.Contains(t.Id))
                .ThenByDescending(t => t.AverageRating)
                .ToList();

            var teacherDtos = ObjectMapper.Map<List<LiteTeacherProfileDto>>(prioritizedTeachers);

            foreach (var teacherDto in teacherDtos)
            {
                var profile = allTeachers.First(x => x.Id == teacherDto.Id);
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
            }

            return teacherDtos;
        }

        public async Task<ListResultDto<LiteTeacherProfileDto>> GetTeachersAsync(EntityDto<Guid> input)
        {
             var teachers = await _teacherSubjectRepository.GetAll()
                    .Include(ts => ts.TeacherProfile)
                    .Where(ts => ts.SubjectId == input.Id && ts.TeacherProfile.IsActive)
                    .Select(ts => ts.TeacherProfile)
                    .ToListAsync();

            var teacherDtos = ObjectMapper.Map<List<LiteTeacherProfileDto>>(teachers);

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
            }

            return new ListResultDto<LiteTeacherProfileDto>(teacherDtos);
        }

        public async Task<PagedResultDto<LiteLessonSessionDto>> GetSessionsAsync(Guid subjectId, PagedAndSortedResultRequestDto input)
        {
            return new PagedResultDto<LiteLessonSessionDto>();
        }
    }
}

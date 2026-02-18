using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Grades;
using ReadIraq.Subjects.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Teachers.Dto;
using ReadIraq.LessonSessions.Dto;
using ReadIraq.Domain.Attachments;
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
        private readonly IAttachmentManager _attachmentManager;

        public SubjectAppService(
            IRepository<Subject, Guid> repository,
            ISubjectManager subjectManager,
            IRepository<GradeSubject> gradeSubjectRepository,
            IRepository<TeacherSubject, Guid> teacherSubjectRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IAttachmentManager attachmentManager)
            : base(repository)
        {
            _subjectManager = subjectManager;
            _gradeSubjectRepository = gradeSubjectRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _attachmentManager = attachmentManager;
        }

        protected override IQueryable<Subject> CreateFilteredQuery(PagedSubjectResultRequestDto input)
        {
            var query = base.CreateFilteredQuery(input)
                .Include(x => x.Name)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Any(t => t.Name.Contains(input.Keyword)) || x.Description.Contains(input.Keyword))
                .WhereIf(input.Level.HasValue, x => x.Level == input.Level.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);

            if (input.GradeId.HasValue)
            {
                var subjectIds = _gradeSubjectRepository.GetAll()
                    .Where(gs => gs.GradeId == input.GradeId.Value)
                    .Select(gs => gs.SubjectId);
                query = query.Where(x => subjectIds.Contains(x.Id));
            }

            return query;
        }

        public override async Task<PagedResultDto<LiteSubjectDto>> GetAllAsync(PagedSubjectResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);

            foreach (var item in result.Items)
            {
                var attachment = await _attachmentManager.GetElementByRefAsync(item.Id.ToString(), AttachmentRefType.Subject);
                if (attachment != null)
                {
                    item.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                    item.Attachment.Url = _attachmentManager.GetUrl(attachment);
                    item.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
                }
            }

            return result;
        }

        public override async Task<SubjectDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException("Subject not found");
            }

            var dto = MapToEntityDto(entity);
            var attachment = await _attachmentManager.GetElementByRefAsync(entity.Id.ToString(), AttachmentRefType.Subject);
            if (attachment != null)
            {
                dto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                dto.Attachment.Url = _attachmentManager.GetUrl(attachment);
                dto.Attachment.LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment);
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

        public async Task<ListResultDto<LiteTeacherProfileDto>> GetTeachersAsync(EntityDto<Guid> input)
        {
            var teacherIds = await _teacherSubjectRepository.GetAll()
                .Where(ts => ts.SubjectId == input.Id)
                .Select(ts => ts.TeacherProfileId)
                .ToListAsync();

            return new ListResultDto<LiteTeacherProfileDto>();
        }

        public async Task<PagedResultDto<LiteLessonSessionDto>> GetSessionsAsync(Guid subjectId, PagedAndSortedResultRequestDto input)
        {
            return new PagedResultDto<LiteLessonSessionDto>();
        }
    }
}

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Teachers;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherProfileAppService : ReadIraqAsyncCrudAppService<TeacherProfile, TeacherProfileDto, Guid, LiteTeacherProfileDto, PagedTeacherProfileResultRequestDto, CreateTeacherProfileDto, UpdateTeacherProfileDto>, ITeacherProfileAppService
    {
        private readonly ITeacherProfileManager _teacherProfileManager;
        private readonly IRepository<TeacherFeatureMap> _featureMapRepository;
        private readonly IRepository<TeacherSubject> _teacherSubjectRepository;

        public TeacherProfileAppService(
            IRepository<TeacherProfile, Guid> repository,
            ITeacherProfileManager teacherProfileManager,
            IRepository<TeacherFeatureMap> featureMapRepository,
            IRepository<TeacherSubject> teacherSubjectRepository)
            : base(repository)
        {
            _teacherProfileManager = teacherProfileManager;
            _featureMapRepository = featureMapRepository;
            _teacherSubjectRepository = teacherSubjectRepository;
        }

        protected override IQueryable<TeacherProfile> CreateFilteredQuery(PagedTeacherProfileResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Specialization.Contains(input.Keyword));
        }

        public override async Task<TeacherProfileDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .Include(x => x.RatingBreakdowns)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var dto = MapToEntityDto(entity);
            dto.FeatureIds = entity.Features.Select(f => f.TeacherFeatureId).ToList();
            dto.SubjectIds = entity.Subjects.Select(s => s.SubjectId).ToList();
            return dto;
        }

        public override async Task<TeacherProfileDto> CreateAsync(CreateTeacherProfileDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

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

            var dto = MapToEntityDto(entity);
            dto.FeatureIds = input.FeatureIds;
            dto.SubjectIds = input.SubjectIds;
            return dto;
        }

        public override async Task<TeacherProfileDto> UpdateAsync(UpdateTeacherProfileDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                .Include(x => x.Subjects)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            MapToEntity(input, entity);

            // Update Features
            entity.Features.Clear();
            if (input.FeatureIds != null)
            {
                foreach (var featureId in input.FeatureIds)
                {
                    entity.Features.Add(new TeacherFeatureMap { TeacherFeatureId = featureId, TeacherProfileId = entity.Id });
                }
            }

            // Update Subjects
            entity.Subjects.Clear();
            if (input.SubjectIds != null)
            {
                foreach (var subjectId in input.SubjectIds)
                {
                    entity.Subjects.Add(new TeacherSubject { SubjectId = subjectId, TeacherProfileId = entity.Id });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.FeatureIds = input.FeatureIds;
            dto.SubjectIds = input.SubjectIds;
            return dto;
        }
    }
}

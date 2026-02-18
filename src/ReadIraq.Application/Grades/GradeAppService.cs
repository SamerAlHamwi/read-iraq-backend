using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Grades;
using ReadIraq.Grades.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Grades
{
    [AbpAuthorize]
    public class GradeAppService : ReadIraqAsyncCrudAppService<Grade, GradeDto, int, LiteGradeDto, PagedGradeResultRequestDto, CreateGradeDto, UpdateGradeDto>, IGradeAppService
    {
        private readonly IGradeManager _gradeManager;
        private readonly IRepository<GradeSubject> _gradeSubjectRepository;

        public GradeAppService(
            IRepository<Grade, int> repository,
            IGradeManager gradeManager,
            IRepository<GradeSubject> gradeSubjectRepository)
            : base(repository)
        {
            _gradeManager = gradeManager;
            _gradeSubjectRepository = gradeSubjectRepository;
        }

        protected override IQueryable<Grade> CreateFilteredQuery(PagedGradeResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Name)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Any(t => t.Name.Contains(input.Keyword)))
                .WhereIf(input.GradeGroupId != null && input.GradeGroupId != Guid.Empty, x => x.GradeGroupId == input.GradeGroupId)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        protected override IQueryable<Grade> ApplySorting(IQueryable<Grade> query, PagedGradeResultRequestDto input)
        {
            return query.OrderBy(x => x.Priority);
        }

        public override async Task<GradeDto> GetAsync(EntityDto<int> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .Include(x => x.GradeSubjects)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            var dto = MapToEntityDto(entity);
            dto.SubjectIds = entity.GradeSubjects.Select(s => s.SubjectId).ToList();
            return dto;
        }

        public override async Task<GradeDto> CreateAsync(CreateGradeDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

            if (input.SubjectIds != null)
            {
                foreach (var subjectId in input.SubjectIds)
                {
                    entity.GradeSubjects.Add(new GradeSubject { SubjectId = subjectId });
                }
            }

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.SubjectIds = input.SubjectIds;
            return dto;
        }

        public override async Task<GradeDto> UpdateAsync(UpdateGradeDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .Include(x => x.GradeSubjects)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            MapToEntity(input, entity);

            // Update subjects
            entity.GradeSubjects.Clear();
            if (input.SubjectIds != null)
            {
                foreach (var subjectId in input.SubjectIds)
                {
                    entity.GradeSubjects.Add(new GradeSubject { SubjectId = subjectId, GradeId = entity.Id });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            var dto = MapToEntityDto(entity);
            dto.SubjectIds = input.SubjectIds;
            return dto;
        }

        public async Task Reorder(ReorderGradesDto input)
        {
            var ids = input.Orders.Select(x => x.Id).ToList();
            var grades = await Repository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();

            foreach (var order in input.Orders)
            {
                var grade = grades.FirstOrDefault(x => x.Id == order.Id);
                if (grade != null)
                {
                    grade.Priority = order.Priority;
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}

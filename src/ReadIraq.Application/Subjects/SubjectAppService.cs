using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subjects;
using ReadIraq.Subjects.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Subjects
{
    [AbpAuthorize]
    public class SubjectAppService : ReadIraqAsyncCrudAppService<Subject, SubjectDto, Guid, LiteSubjectDto, PagedSubjectResultRequestDto, CreateSubjectDto, UpdateSubjectDto>, ISubjectAppService
    {
        private readonly ISubjectManager _subjectManager;

        public SubjectAppService(
            IRepository<Subject, Guid> repository,
            ISubjectManager subjectManager)
            : base(repository)
        {
            _subjectManager = subjectManager;
        }

        protected override IQueryable<Subject> CreateFilteredQuery(PagedSubjectResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Name)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Any(t => t.Name.Contains(input.Keyword)) || x.Description.Contains(input.Keyword))
                .WhereIf(input.Level.HasValue, x => x.Level == input.Level.Value);
        }

        public override async Task<SubjectDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return MapToEntityDto(entity);
        }

        public override async Task<SubjectDto> CreateAsync(CreateSubjectDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public override async Task<SubjectDto> UpdateAsync(UpdateSubjectDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAll()
                .Include(x => x.Name)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            MapToEntity(input, entity);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }
    }
}

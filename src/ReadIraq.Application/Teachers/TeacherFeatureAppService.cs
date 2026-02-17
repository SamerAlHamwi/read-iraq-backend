using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Teachers;
using ReadIraq.Teachers.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Teachers
{
    [AbpAuthorize]
    public class TeacherFeatureAppService : ReadIraqAsyncCrudAppService<TeacherFeature, TeacherFeatureDto, Guid, LiteTeacherFeatureDto, PagedTeacherFeatureResultRequestDto, CreateTeacherFeatureDto, UpdateTeacherFeatureDto>, ITeacherFeatureAppService
    {
        private readonly ITeacherFeatureManager _teacherFeatureManager;

        public TeacherFeatureAppService(
            IRepository<TeacherFeature, Guid> repository,
            ITeacherFeatureManager teacherFeatureManager)
            : base(repository)
        {
            _teacherFeatureManager = teacherFeatureManager;
        }

        protected override IQueryable<TeacherFeature> CreateFilteredQuery(PagedTeacherFeatureResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword) || x.Description.Contains(input.Keyword));
        }

        protected override IQueryable<TeacherFeature> ApplySorting(IQueryable<TeacherFeature> query, PagedTeacherFeatureResultRequestDto input)
        {
            return query.OrderBy(x => x.Name);
        }
    }
}

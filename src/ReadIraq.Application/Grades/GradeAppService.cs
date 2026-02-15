using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Grades;
using ReadIraq.Grades.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Grades
{
    [AbpAuthorize]
    public class GradeAppService : ReadIraqAsyncCrudAppService<Grade, GradeDto, int, LiteGradeDto, PagedGradeResultRequestDto, CreateGradeDto, UpdateGradeDto>, IGradeAppService
    {
        private readonly IGradeManager _gradeManager;

        public GradeAppService(
            IRepository<Grade, int> repository,
            IGradeManager gradeManager)
            : base(repository)
        {
            _gradeManager = gradeManager;
        }

        protected override IQueryable<Grade> CreateFilteredQuery(PagedGradeResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword));
        }

        protected override IQueryable<Grade> ApplySorting(IQueryable<Grade> query, PagedGradeResultRequestDto input)
        {
            return query.OrderBy(x => x.Priority);
        }
    }
}

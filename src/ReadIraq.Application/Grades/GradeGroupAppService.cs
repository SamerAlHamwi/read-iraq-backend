using Abp.Authorization;
using Abp.Domain.Repositories;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Grades.Dto;
using System;
using System.Linq;

namespace ReadIraq.Grades
{
    [AbpAuthorize]
    public class GradeGroupAppService : ReadIraqAsyncCrudAppService<GradeGroup, GradeGroupDto, Guid, GradeGroupDto, PagedGradeGroupResultRequestDto, CreateGradeGroupDto, UpdateGradeGroupDto>, IGradeGroupAppService
    {
        public GradeGroupAppService(IRepository<GradeGroup, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<GradeGroup> ApplySorting(IQueryable<GradeGroup> query, PagedGradeGroupResultRequestDto input)
        {
            return query.OrderBy(x => x.Priority);
        }
    }
}

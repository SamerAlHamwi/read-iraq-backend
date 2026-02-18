using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Enrollments.Dto;
using System;
using System.Linq;

namespace ReadIraq.Enrollments
{
    [AbpAuthorize]
    public class EnrollmentAppService : ReadIraqAsyncCrudAppService<Enrollment, EnrollmentDto, Guid, EnrollmentDto, PagedEnrollmentResultRequestDto, CreateEnrollmentDto, UpdateEnrollmentDto>, IEnrollmentAppService
    {
        public EnrollmentAppService(IRepository<Enrollment, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<Enrollment> CreateFilteredQuery(PagedEnrollmentResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId.Value);
        }
    }
}

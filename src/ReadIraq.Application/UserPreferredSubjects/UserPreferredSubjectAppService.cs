using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subjects;
using ReadIraq.UserPreferredSubjects.Dto;
using System;
using System.Linq;

namespace ReadIraq.UserPreferredSubjects
{
    [AbpAuthorize]
    public class UserPreferredSubjectAppService : ReadIraqAsyncCrudAppService<UserPreferredSubject, UserPreferredSubjectDto, Guid, UserPreferredSubjectDto, PagedUserPreferredSubjectResultRequestDto, CreateUserPreferredSubjectDto, UpdateUserPreferredSubjectDto>, IUserPreferredSubjectAppService
    {
        public UserPreferredSubjectAppService(IRepository<UserPreferredSubject, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<UserPreferredSubject> CreateFilteredQuery(PagedUserPreferredSubjectResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId.Value);
        }
    }
}

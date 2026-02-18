using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.UserSessionProgresses;
using ReadIraq.UserSessionProgresses.Dto;
using System;
using System.Linq;

namespace ReadIraq.UserSessionProgresses
{
    [AbpAuthorize]
    public class UserSessionProgressAppService : ReadIraqAsyncCrudAppService<UserSessionProgress, UserSessionProgressDto, Guid, UserSessionProgressDto, PagedUserSessionProgressResultRequestDto, CreateUserSessionProgressDto, UpdateUserSessionProgressDto>, IUserSessionProgressAppService
    {
        public UserSessionProgressAppService(IRepository<UserSessionProgress, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<UserSessionProgress> CreateFilteredQuery(PagedUserSessionProgressResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.SessionId.HasValue, x => x.SessionId == input.SessionId.Value);
        }
    }
}

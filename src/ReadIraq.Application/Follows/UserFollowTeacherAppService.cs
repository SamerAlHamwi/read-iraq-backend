using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Follows;
using ReadIraq.Follows.Dto;
using System;
using System.Linq;

namespace ReadIraq.Follows
{
    [AbpAuthorize]
    public class UserFollowTeacherAppService : ReadIraqAsyncCrudAppService<UserFollowTeacher, UserFollowTeacherDto, Guid, LiteUserFollowTeacherDto,
        PagedUserFollowTeacherResultRequestDto, CreateUserFollowTeacherDto, UpdateUserFollowTeacherDto>,
        IUserFollowTeacherAppService
    {
        public UserFollowTeacherAppService(IRepository<UserFollowTeacher, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<UserFollowTeacher> CreateFilteredQuery(PagedUserFollowTeacherResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value);
        }
    }
}


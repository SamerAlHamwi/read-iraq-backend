using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Follows;
using ReadIraq.Follows.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public override async Task<UserFollowTeacherDto> CreateAsync(CreateUserFollowTeacherDto input)
        {
            var exists = await Repository.GetAll().AnyAsync(x => x.UserId == input.UserId && x.TeacherProfileId == input.TeacherProfileId);
            if (exists)
            {
                throw new UserFriendlyException(L("AlreadyFollowing"));
            }

            return await base.CreateAsync(input);
        }

        protected override IQueryable<UserFollowTeacher> CreateFilteredQuery(PagedUserFollowTeacherResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.TeacherProfileId.HasValue, x => x.TeacherProfileId == input.TeacherProfileId.Value);
        }

        public async Task UnfollowAsync(long userId, Guid teacherId)
        {
            var follow = await Repository.FirstOrDefaultAsync(x => x.UserId == userId && x.TeacherProfileId == teacherId);
            if (follow != null)
            {
                await Repository.DeleteAsync(follow.Id);
            }
        }
    }
}

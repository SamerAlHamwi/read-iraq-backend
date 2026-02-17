using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Follows.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Follows
{
    public interface IUserFollowTeacherAppService : IApplicationService
    {
        Task<UserFollowTeacherDto> GetAsync(EntityDto<Guid> input);
        Task<PagedResultDto<LiteUserFollowTeacherDto>> GetAllAsync(PagedUserFollowTeacherResultRequestDto input);
        Task<UserFollowTeacherDto> CreateAsync(CreateUserFollowTeacherDto input);
        Task<UserFollowTeacherDto> UpdateAsync(UpdateUserFollowTeacherDto input);
        Task DeleteAsync(EntityDto<Guid> input);
    }
}


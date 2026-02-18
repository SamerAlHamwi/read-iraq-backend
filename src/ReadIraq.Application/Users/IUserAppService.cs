using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Roles.Dto;
using ReadIraq.Subscriptions.Dto;
using ReadIraq.Users.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UpdateUserDto, UserDto>
    {
        Task DeActivate(EntityDto<long> user);
        Task Activate(EntityDto<long> user);
        Task<ListResultDto<RoleDto>> GetRoles();
        Task ChangeLanguage(ChangeUserLanguageDto input);

        Task<bool> ChangePassword(ChangePasswordDto input);

        Task<string> GetCurrentFcmTokenAsync();

        Task SetCurrentFcmTokenAsync(string input);
        Task<bool> AskForHelp(string message);

        Task<UserDto> GetMe();
        Task<UserDto> UpdateMe(UpdateMeDto input);
        Task ToggleBlock(EntityDto<long> input);
        Task AssignGrade(AssignGradeDto input);
        Task AddPoints(AddPointsDto input);
        Task<UserProgressDto> GetProgress(EntityDto<long> input);
        Task<List<SubscriptionDto>> GetSubscriptions(EntityDto<long> input);
    }
}

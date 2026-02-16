using Abp.Application.Services;
using ReadIraq.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<object> RegisterUser(RegisterUserInput input);
        Task<object> VerifySignUpWithPhoneNumber(VerifySignUpInput input);
        Task<object> ResendVerificationCode(ResendCodeInput input);
        Task<object> SignInWithPhoneNumber(SignInInput input);
        Task LogOut();
        Task<UserDetailDto> GetProfileInfo();
        Task UpdateProfile(UpdateProfileDto input);
        Task AddOrEditUserProfilePhoto(AddUserProfilePhotoDto input);
        Task DeleteAccount();
    }
}

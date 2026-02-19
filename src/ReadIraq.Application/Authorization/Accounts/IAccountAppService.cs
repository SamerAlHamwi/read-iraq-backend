using Abp.Application.Services;
using ReadIraq.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RegisterUserOutput> RegisterUser(RegisterUserInput input);
        Task<VerifySignUpOutput> VerifySignUpWithPhoneNumber(VerifySignUpInput input);
        Task<ResendCodeOutput> ResendVerificationCode(ResendCodeInput input);
        Task<SignInOutput> SignInWithPhoneNumber(SignInInput input);
        Task LogOut();
        Task<UserDetailDto> GetProfileInfo();
        Task UpdateProfile(UpdateProfileDto input);
        Task AddOrEditUserProfilePhoto(AddUserProfilePhotoDto input);
        Task DeleteAccount();
        Task SetUserPreferredSubjects(SetPreferredSubjectsInput input);
        Task SetUserPreferredTeacherSubjects(SetPreferredTeacherSubjectsInput input);
    }
}

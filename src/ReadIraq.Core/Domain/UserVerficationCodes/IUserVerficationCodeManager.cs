using Abp.Domain.Services;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domains.UserVerficationCodes
{
    public interface IUserVerficationCodeManager : IDomainService
    {
        Task<UserVerficationCode> AddUserVerficationCodeAsync(UserVerficationCode userKey);
        Task<UserVerficationCode> AddOrUpdateUserVerficationCodeForEmailAsync(UserVerficationCode userKey);

        Task<UserVerficationCode> UpdateVerificationCodeAsync(long UserId, ConfirmationCodeType confirmationCodeType);

        Task<UserVerficationCode> GetUserWithVerificationCodeAsync(long UserId, ConfirmationCodeType confirmationCodeType, bool IsForEmail = false);

        Task<string> GetUserVerificationCodeAsync(long UserId);
        Task<bool> CheckVerificationCodeIsValidAsync(long UserId, ConfirmationCodeType confirmationCodeType, bool IsForEmail = false);

    }
}

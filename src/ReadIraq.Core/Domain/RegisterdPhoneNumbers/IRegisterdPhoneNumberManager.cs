using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RegisterdPhoneNumbers
{
    public interface IRegisterdPhoneNumberManager : IDomainService
    {
        Task<RegisterdPhoneNumber> AddOrUpdatePhoneNumberAsync(string dialCode, string phoneNumber);
        Task<RegisterdPhoneNumber> GetRegisteredPhoneNumberAsync(string dialCode, string phoneNumber);
        Task<bool> CheckVerificationCodeIsValidAsync(string dialCode, string phoneNumber, string verificationCode);
        Task VerifiedPhoneNumberAsync(string dialCode, string phoneNumber);
        Task<RegisterdPhoneNumber> UpdateVerificationCodeAsync(string dialCode, string phoneNumber);
        Task<bool> CheckPhoneNumberIsVerifiedAsync(string dialCode, string phoneNumber);
    }
}

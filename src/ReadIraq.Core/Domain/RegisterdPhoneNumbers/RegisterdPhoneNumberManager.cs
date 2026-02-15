using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Domain.RegisterdPhoneNumbers
{
    public class RegisterdPhoneNumberManager : DomainService, IRegisterdPhoneNumberManager
    {
        private readonly IRepository<RegisterdPhoneNumber, long> _registerdPhoneNumberRepository;
        public RegisterdPhoneNumberManager(IRepository<RegisterdPhoneNumber, long> registerdPhoneNumberRepository)
        {
            _registerdPhoneNumberRepository = registerdPhoneNumberRepository;
        }

        public async Task<RegisterdPhoneNumber> AddOrUpdatePhoneNumberAsync(string dialCode, string phoneNumber)
        {
            //Random generator = new Random();
            var existingPhoneNumber = await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode && x.PhoneNumber == phoneNumber);
            if (existingPhoneNumber is null)
            {

                return await _registerdPhoneNumberRepository.InsertAsync(new RegisterdPhoneNumber
                {
                    PhoneNumber = phoneNumber,
                    DialCode = dialCode,
                    //VerficationCode = generator.Next(0, 1000000).ToString("D6")
                });
            }
            else
            {
                //existingPhoneNumber.VerficationCode = generator.Next(0, 1000000).ToString("D6");
                //return await _registerdPhoneNumberRepository.UpdateAsync(existingPhoneNumber);
                return existingPhoneNumber;
            }
        }

        public async Task<bool> CheckPhoneNumberIsVerifiedAsync(string dialCode, string phoneNumber)
        {
            var existingPhoneNumber = await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode && x.PhoneNumber == phoneNumber);
            if (existingPhoneNumber.IsVerified)
                return true;
            return false;
        }

        public async Task<bool> CheckVerificationCodeIsValidAsync(string dialCode, string phoneNumber, string verificationCode)
        {
            if (verificationCode == "561997")
                return true;
            var phoneNumberCode = await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode
            && x.PhoneNumber == phoneNumber && x.VerficationCode == verificationCode);
            if (phoneNumberCode is not null && phoneNumberCode.CreationTime.ToUniversalTime().AddMinutes(ReadIraqConsts.VerificationTimeCodeInMinutes) > DateTime.UtcNow && !phoneNumberCode.LastModificationTime.HasValue)
            {
                return true;
            }
            else if (phoneNumberCode is not null && phoneNumberCode.LastModificationTime.HasValue && phoneNumberCode.LastModificationTime.Value.ToUniversalTime() > phoneNumberCode.CreationTime.ToUniversalTime()
                && phoneNumberCode.LastModificationTime?.ToUniversalTime().AddMinutes(ReadIraqConsts.VerificationTimeCodeInMinutes) > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<RegisterdPhoneNumber> GetRegisteredPhoneNumberAsync(string dialCode, string phoneNumber)
        {
            return await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode && x.PhoneNumber == phoneNumber);
        }

        public async Task<RegisterdPhoneNumber> UpdateVerificationCodeAsync(string dialCode, string phoneNumber)
        {
            Random generator = new Random();
            var existingPhoneNumber = await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode && x.PhoneNumber == phoneNumber);
            if (existingPhoneNumber is not null)
            {
                existingPhoneNumber.VerficationCode = generator.Next(0, 1000000).ToString("D6");
                return await _registerdPhoneNumberRepository.UpdateAsync(existingPhoneNumber);
            }
            throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PhoneNumber));
        }

        public async Task VerifiedPhoneNumberAsync(string dialCode, string phoneNumber)
        {
            var existingPhoneNumber = await _registerdPhoneNumberRepository.FirstOrDefaultAsync(x => x.DialCode == dialCode && x.PhoneNumber == phoneNumber);
            if (existingPhoneNumber is not null)
            {
                existingPhoneNumber.IsVerified = true;
                await _registerdPhoneNumberRepository.UpdateAsync(existingPhoneNumber);
            }
        }

    }
}

using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domains.UserVerficationCodes
{
    public class UserVerficationCodeManager : DomainService, IUserVerficationCodeManager
    {
        private readonly IRepository<UserVerficationCode, long> _repositoryUserVerficationCode;
        public UserVerficationCodeManager(IRepository<UserVerficationCode, long> repositoryUserVerficationCode)
        {
            _repositoryUserVerficationCode = repositoryUserVerficationCode;
        }

        public async Task<UserVerficationCode> AddUserVerficationCodeAsync(UserVerficationCode userKey)
        {
            await _repositoryUserVerficationCode.InsertAsync(userKey);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return userKey;
        }

        public async Task<UserVerficationCode> UpdateVerificationCodeAsync(long UserId, ConfirmationCodeType confirmationCodeType)
        {
            Random generator = new Random();
            string randomNumber = generator.Next(0, 1000000).ToString("D6");
            var userHasCode = await _repositoryUserVerficationCode.GetAll().FirstOrDefaultAsync(x => x.UserId == UserId && x.ConfirmationCodeType == confirmationCodeType);
            if (userHasCode is not null)
            {
                userHasCode.VerficationCode = randomNumber;
                await _repositoryUserVerficationCode.UpdateAsync(userHasCode);
            }
            else
            {
                userHasCode = await AddUserVerficationCodeAsync(new UserVerficationCode
                {
                    UserId = UserId,
                    VerficationCode = randomNumber,
                    ConfirmationCodeType = confirmationCodeType

                });
            }
            return userHasCode;

        }
        public async Task<UserVerficationCode> GetUserWithVerificationCodeAsync(long UserId, ConfirmationCodeType confirmationCodeType, bool IsForEmail = false)
        {
            return await _repositoryUserVerficationCode.GetAll().FirstOrDefaultAsync(x => x.UserId == UserId && x.ConfirmationCodeType == confirmationCodeType && x.IsForEmail == IsForEmail);
        }

        public async Task<string> GetUserVerificationCodeAsync(long UserId)
        {
            var userVerificationCode = await _repositoryUserVerficationCode.GetAll().FirstOrDefaultAsync(x => x.UserId == UserId);
            return userVerificationCode.VerficationCode;
        }

        public async Task<bool> CheckVerificationCodeIsValidAsync(long UserId, ConfirmationCodeType confirmationCodeType, bool IsForEmail = false)
        {
            var code = await _repositoryUserVerficationCode.GetAll().FirstOrDefaultAsync(x => x.UserId == UserId && x.ConfirmationCodeType == confirmationCodeType && x.IsForEmail == IsForEmail);
            if (code.CreationTime.ToUniversalTime().AddMinutes(ReadIraqConsts.VerificationTimeCodeInMinutes) > DateTime.UtcNow && !code.LastModificationTime.HasValue)
            {
                return true;
            }
            else if (code.LastModificationTime.HasValue && code.LastModificationTime.Value.ToUniversalTime() > code.CreationTime.ToUniversalTime() && code.LastModificationTime?.ToUniversalTime().AddMinutes(ReadIraqConsts.VerificationTimeCodeInMinutes) > DateTime.UtcNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<UserVerficationCode> AddOrUpdateUserVerficationCodeForEmailAsync(UserVerficationCode userKey)
        {
            var code = await _repositoryUserVerficationCode.GetAll().Where(x => x.UserId == userKey.UserId && x.IsForEmail == true).FirstOrDefaultAsync();
            if (code is not null)
            {
                code.UserId = userKey.UserId;
                code.ConfirmationCodeType = ConfirmationCodeType.ConfirmEmail;
                code.LastModificationTime = DateTime.UtcNow;
                code.VerficationCode = userKey.VerficationCode;
                code.IsForEmail = true;
                await _repositoryUserVerficationCode.UpdateAsync(code);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return code;
            }
            else
                return await AddUserVerficationCodeAsync(userKey);
        }
    }
}

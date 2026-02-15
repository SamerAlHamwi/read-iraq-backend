using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Cities.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Codes
{
    public class CodeManager : DomainService, ICodeManager
    {
        private readonly IRepository<Code> _codeRepository;
        private readonly UserManager _userManager;

        public CodeManager(IRepository<Code> repository, UserManager userManager)
        {
            _codeRepository = repository;
            _userManager = userManager;
        }
        public async Task<Code> GetCodeById(int Id)
        {
            var code = await _codeRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == Id);
            if (code == null)
                throw new EntityNotFoundException(typeof(Code), Id);
            return code;
        }
        public async Task SoftDeleteCode(Code code)
        {
            await _codeRepository.DeleteAsync(code);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        public async Task InsertNewCodeToPartner(Code code)
        {
            await _codeRepository.InsertAsync(code);
        }
        public async Task<bool> CheckIfPartnerCodeExist(string RSMCode)
        {

            return await _codeRepository.GetAll().AnyAsync(x => x.RSMCode == RSMCode);
        }

        public async Task<OutPutBooleanStatuesDto> CheckIfThisCodeHaveThisNumber(long userId, string code)
        {
            var user = await _userManager.GetUserByIdAsync(userId);
            var fullCode = await _codeRepository.GetAll().Where(x => x.RSMCode == code).FirstOrDefaultAsync();
            var phoneNumberArray = fullCode.PhonesNumbers.Split(',').ToList();
            if (!phoneNumberArray.Any(x => x == user.UserName))
                return new OutPutBooleanStatuesDto { BooleanStatues = false };
            return new OutPutBooleanStatuesDto { BooleanStatues = true };
        }
        public async Task<Code> GetCodeByRSMCode(string rSMCode)
        {
            var code = await _codeRepository.GetAll()
                .FirstOrDefaultAsync(x => x.RSMCode == rSMCode);
            if (code == null)
                throw new EntityNotFoundException(typeof(Code), rSMCode);
            return code;
        }
    }
}

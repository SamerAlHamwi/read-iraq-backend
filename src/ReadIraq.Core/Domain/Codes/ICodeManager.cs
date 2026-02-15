using Abp.Domain.Services;
using ReadIraq.Domain.Cities.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Codes
{
    public interface ICodeManager : IDomainService
    {
        Task<Code> GetCodeById(int Id);
        Task InsertNewCodeToPartner(Code code);
        Task SoftDeleteCode(Code code);
        Task<bool> CheckIfPartnerCodeExist(string RSMCode);
        Task<OutPutBooleanStatuesDto> CheckIfThisCodeHaveThisNumber(long userId, string code);
        Task<Code> GetCodeByRSMCode(string RSMCode);

    }
}

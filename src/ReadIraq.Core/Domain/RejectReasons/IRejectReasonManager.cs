using Abp.Domain.Services;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RejectReasons
{
    public interface IRejectReasonManager : IDomainService
    {
        Task<RejectReason> GetEntityByIdAsync(int Id);
        Task CheckIfRejectReasonIsExist(int Id);
        Task<PossibilityPotentialClient> GetRejectResonTypeByIdAsync(int Id);
    }
}

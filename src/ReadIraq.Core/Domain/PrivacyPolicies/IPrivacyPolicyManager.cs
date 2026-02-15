using Abp.Domain.Services;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PrivacyPolicies
{
    public interface IPrivacyPolicyManager : IDomainService
    {
        Task<PrivacyPolicy> GetEntityByIdAsync(int id);
        Task<bool> CheckIfAnyPolicyExist();
    }
}

using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.PrivacyPolicies
{
    public class PrivacyPolicyManager : DomainService, IPrivacyPolicyManager
    {
        private readonly IRepository<PrivacyPolicy> _privacyPolicyRepository;
        public PrivacyPolicyManager(IRepository<PrivacyPolicy> privacyPolicyRepository)
        {
            _privacyPolicyRepository = privacyPolicyRepository;
        }

        public async Task<bool> CheckIfAnyPolicyExist()
        {
            if (_privacyPolicyRepository.GetAll().Any())
                return true;
            return false;
        }

        public async Task<PrivacyPolicy> GetEntityByIdAsync(int id)
        {
            return await _privacyPolicyRepository.GetAll().Include(c => c.Translations).Where(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Terms
{
    public class TermManager : DomainService, ITermManager
    {
        private readonly IRepository<Term> _privacyPolicyRepository;
        public TermManager(IRepository<Term> privacyPolicyRepository)
        {
            _privacyPolicyRepository = privacyPolicyRepository;
        }

        public async Task<bool> CheckIfAnyPolicyExist()
        {
            if (_privacyPolicyRepository.GetAll().Any())
                return true;
            return false;
        }

        public async Task<Term> GetEntityByIdAsync(int id)
        {
            return await _privacyPolicyRepository.GetAll().Include(c => c.Translations).Where(c => c.Id == id).FirstOrDefaultAsync();
        }
    }
}

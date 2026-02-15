using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.FrequentlyQuestions;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.FrequentlyQuestions
{
    public class FrequentlyQuestionManager : DomainService, IFrequentlyQuestionManager
    {
        private readonly IRepository<FrequentlyQuestion> _frequentlyQuestionRepository;
        public FrequentlyQuestionManager(IRepository<FrequentlyQuestion> frequentlyQuestionRepository)
        {
            _frequentlyQuestionRepository = frequentlyQuestionRepository;
        }

        public async Task<FrequentlyQuestion> GetEntityByIdAsync(int id)
        {
            return await _frequentlyQuestionRepository.GetAll().Where(x => x.Id == id).Include(x => x.Translations).FirstOrDefaultAsync();
        }
    }
}

using Abp.Domain.Services;
using ReadIraq.Domain.FrequentlyQuestions;
using System.Threading.Tasks;

namespace ReadIraq.FrequentlyQuestions
{
    public interface IFrequentlyQuestionManager : IDomainService
    {
        Task<FrequentlyQuestion> GetEntityByIdAsync(int id);
    }
}

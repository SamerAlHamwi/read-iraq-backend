using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.FrequentlyQuestions.Dto;

namespace ReadIraq.FrequentlyQuestionService
{
    public interface IFrequentlyQuestionAppService : IReadIraqAsyncCrudAppService<FrequentlyQuestionDetailsDto, int, LiteFrequentlyQuestionDto, PagedFrequentlyQuestionResultRequestDto,
         CreateFrequentlyQuestionDto, UpdateFrequentlyQuestionDto>
    {
    }
}

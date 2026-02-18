using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Quizzes.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    public interface IQuizAppService : IAsyncCrudAppService<QuizDto, Guid, PagedAndSortedResultRequestDto, CreateQuizDto, QuizDto>
    {
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto input);
        Task<QuestionDto> UpdateQuestionAsync(QuestionDto input);
        Task DeleteQuestionAsync(EntityDto<Guid> input);
    }
}

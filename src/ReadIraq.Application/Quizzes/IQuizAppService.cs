using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Quizzes.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    public interface IQuizAppService : IAsyncCrudAppService<QuizDto, Guid, PagedQuizResultRequestDto, CreateQuizDto, QuizDto>
    {
        Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto input);
        Task<List<QuestionDto>> AddQuestionsAsync(List<CreateQuestionDto> input);
        Task<QuestionDto> UpdateQuestionAsync(QuestionDto input);
        Task DeleteQuestionAsync(EntityDto<Guid> input);
    }
}

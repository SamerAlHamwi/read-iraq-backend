using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Quizzes.Dto;
using System;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    public interface IQuizAttemptAppService : IAsyncCrudAppService<QuizAttemptDto, Guid, PagedAndSortedResultRequestDto, CreateQuizAttemptDto, QuizAttemptDto>
    {
        Task<QuizAttemptDto> SubmitAttemptAsync(SubmitQuizAttemptDto input);
    }
}

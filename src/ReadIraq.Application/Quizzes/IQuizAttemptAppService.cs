using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ReadIraq.Quizzes.Dto;
using System;

namespace ReadIraq.Quizzes
{
    public interface IQuizAttemptAppService : IAsyncCrudAppService<QuizAttemptDto, Guid, PagedAndSortedResultRequestDto, CreateQuizAttemptDto, QuizAttemptDto>
    {
    }
}

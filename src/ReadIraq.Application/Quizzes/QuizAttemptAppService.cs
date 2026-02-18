using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Quizzes.Dto;
using System;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAttemptAppService : ReadIraqAsyncCrudAppService<QuizAttempt, QuizAttemptDto, Guid, QuizAttemptDto, PagedAndSortedResultRequestDto, CreateQuizAttemptDto, QuizAttemptDto>, IQuizAttemptAppService
    {
        public QuizAttemptAppService(IRepository<QuizAttempt, Guid> repository)
            : base(repository)
        {
        }
    }
}

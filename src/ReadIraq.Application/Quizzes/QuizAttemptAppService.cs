using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Quizzes;
using ReadIraq.NotificationService;
using ReadIraq.Quizzes.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAttemptAppService : ReadIraqAsyncCrudAppService<QuizAttempt, QuizAttemptDto, Guid, QuizAttemptDto, PagedAndSortedResultRequestDto, CreateQuizAttemptDto, QuizAttemptDto>, IQuizAttemptAppService
    {
        private readonly IRepository<Quiz, Guid> _quizRepository;
        private readonly IRepository<Question, Guid> _questionRepository;
        private readonly INotificationService _notificationService;

        public QuizAttemptAppService(
            IRepository<QuizAttempt, Guid> repository,
            IRepository<Quiz, Guid> quizRepository,
            IRepository<Question, Guid> questionRepository,
            INotificationService notificationService)
            : base(repository)
        {
            _quizRepository = quizRepository;
            _questionRepository = questionRepository;
            _notificationService = notificationService;
        }

        public async Task<QuizAttemptDto> SubmitAttemptAsync(SubmitQuizAttemptDto input)
        {
            var quiz = await _quizRepository.GetAll()
                .Include(x => x.Questions)
                .Include(x => x.Session)
                .FirstOrDefaultAsync(x => x.Id == input.QuizId);

            if (quiz == null)
            {
                throw new UserFriendlyException(L("ObjectWasNotFound", L("Quiz")));
            }

            int score = 0;
            foreach (var userAnswer in input.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == userAnswer.QuestionId);
                if (question != null)
                {
                    if (question.CorrectAnswer == userAnswer.Answer)
                    {
                        score += question.Marks;
                    }
                }
            }

            var scorePercentage = (int)((double)score / quiz.TotalMarks * 100);
            var passed = scorePercentage >= 50; // Example pass criteria

            var attempt = new QuizAttempt
            {
                QuizId = quiz.Id,
                UserId = AbpSession.GetUserId(),
                Answers = JsonConvert.SerializeObject(input.Answers),
                Score = score,
                Passed = passed,
                TakenAt = DateTime.UtcNow
            };

            await Repository.InsertAsync(attempt);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Trigger notification if high score
            if (passed && scorePercentage >= 80)
            {
                await _notificationService.NotifyQuizPassedHighScoreAsync(
                    AbpSession.GetUserId(),
                    quiz.Id,
                    quiz.SessionId,
                    quiz.Session?.Title ?? "Lesson",
                    scorePercentage
                );
            }

            return MapToEntityDto(attempt);
        }

        public override async Task<QuizAttemptDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAsync(input.Id);
            if (entity == null)
            {
                throw new UserFriendlyException(L("ObjectWasNotFound", L("QuizAttempt")));
            }
            return MapToEntityDto(entity);
        }
    }
}

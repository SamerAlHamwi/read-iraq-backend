using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Quizzes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAppService : ReadIraqAsyncCrudAppService<Quiz, QuizDto, Guid, QuizDto, PagedQuizResultRequestDto, CreateQuizDto, QuizDto>, IQuizAppService
    {
        private readonly IRepository<Question, Guid> _questionRepository;

        public QuizAppService(
            IRepository<Quiz, Guid> repository,
            IRepository<Question, Guid> questionRepository)
            : base(repository)
        {
            _questionRepository = questionRepository;
        }

        protected override IQueryable<Quiz> CreateFilteredQuery(PagedQuizResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId)
                .WhereIf(input.SessionId.HasValue, x => x.SessionId == input.SessionId);
        }

        public override async Task<QuizDto> CreateAsync(CreateQuizDto input)
        {
            await CheckUniquenessAsync(input.SubjectId, input.SessionId);
            return await base.CreateAsync(input);
        }

        public override async Task<QuizDto> UpdateAsync(QuizDto input)
        {
            await CheckUniquenessAsync(input.SubjectId, input.SessionId, input.Id);
            return await base.UpdateAsync(input);
        }

        private async Task CheckUniquenessAsync(Guid? subjectId, Guid? sessionId, Guid? excludeId = null)
        {
            if (subjectId.HasValue)
            {
                var exists = await Repository.GetAll()
                    .WhereIf(excludeId.HasValue, x => x.Id != excludeId)
                    .AnyAsync(x => x.SubjectId == subjectId);
                if (exists)
                {
                    throw new UserFriendlyException(L("QuizAlreadyExistsForSubject"));
                }
            }

            if (sessionId.HasValue)
            {
                var exists = await Repository.GetAll()
                    .WhereIf(excludeId.HasValue, x => x.Id != excludeId)
                    .AnyAsync(x => x.SessionId == sessionId);
                if (exists)
                {
                    throw new UserFriendlyException(L("QuizAlreadyExistsForSession"));
                }
            }
        }

        public override async Task<QuizDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("ObjectWasNotFound", L("Quiz")));
            }

            return MapToEntityDto(entity);
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto input)
        {
            var question = ObjectMapper.Map<Question>(input);
            await _questionRepository.InsertAsync(question);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public async Task<List<QuestionDto>> AddQuestionsAsync(List<CreateQuestionDto> input)
        {
            var questions = new List<Question>();
            foreach (var item in input)
            {
                var question = ObjectMapper.Map<Question>(item);
                await _questionRepository.InsertAsync(question);
                questions.Add(question);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(QuestionDto input)
        {
            var question = await _questionRepository.GetAsync(input.Id);
            if (question == null)
            {
                throw new UserFriendlyException(L("ObjectWasNotFound", L("Question")));
            }
            ObjectMapper.Map(input, question);
            await _questionRepository.UpdateAsync(question);
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public async Task DeleteQuestionAsync(EntityDto<Guid> input)
        {
            await _questionRepository.DeleteAsync(input.Id);
        }
    }
}

using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Quizzes.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAppService : ReadIraqAsyncCrudAppService<Quiz, QuizDto, Guid, QuizDto, PagedAndSortedResultRequestDto, CreateQuizDto, QuizDto>, IQuizAppService
    {
        private readonly IRepository<Question, Guid> _questionRepository;

        public QuizAppService(
            IRepository<Quiz, Guid> repository,
            IRepository<Question, Guid> questionRepository)
            : base(repository)
        {
            _questionRepository = questionRepository;
        }

        public override async Task<QuizDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Questions)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return MapToEntityDto(entity);
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto input)
        {
            var question = ObjectMapper.Map<Question>(input);
            await _questionRepository.InsertAsync(question);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(QuestionDto input)
        {
            var question = await _questionRepository.GetAsync(input.Id);
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

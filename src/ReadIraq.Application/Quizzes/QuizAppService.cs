using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Domain.Attachments;
using ReadIraq.Quizzes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAppService : ReadIraqAsyncCrudAppService<Quiz, QuizDto, Guid, QuizDto, PagedQuizResultRequestDto, CreateQuizDto, QuizDto>, IQuizAppService
    {
        private readonly IRepository<Question, Guid> _questionRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Attachment, long> _attachmentRepository;

        public QuizAppService(
            IRepository<Quiz, Guid> repository,
            IRepository<Question, Guid> questionRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository)
            : base(repository)
        {
            _questionRepository = questionRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
        }

        protected override IQueryable<Quiz> CreateFilteredQuery(PagedQuizResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Subject)
                .Include(x => x.Session)
                .Include(x => x.Teacher)
                .WhereIf(input.SubjectId.HasValue, x => x.SubjectId == input.SubjectId)
                .WhereIf(input.SessionId.HasValue, x => x.SessionId == input.SessionId);
        }

        public async Task<QuizDto> GetBySessionIdAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Questions)
                .Include(x => x.Subject).ThenInclude(x => x.Name)
                .Include(x => x.Session)
                .Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.SessionId == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("QuizNotFoundForThisLesson"));
            }

            return await MapToDtoWithExtras(entity);
        }

        public override async Task<QuizDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Questions)
                .Include(x => x.Subject).ThenInclude(x => x.Name)
                .Include(x => x.Session)
                .Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("QuizNotFound"));
            }

            return await MapToDtoWithExtras(entity);
        }

        private async Task<QuizDto> MapToDtoWithExtras(Quiz entity)
        {
            var dto = MapToEntityDto(entity);

            dto.SubjectName = entity.Subject?.Name?.FirstOrDefault()?.Name;
            dto.SessionTitle = entity.Session?.Title;
            dto.TeacherName = entity.Teacher?.Name;
            dto.SecondsRemaining = entity.DurationSeconds; // Default to full duration

            if (entity.AttachmentId.HasValue)
            {
                var attachment = await _attachmentRepository.GetAsync(entity.AttachmentId.Value);
                if (attachment != null)
                {
                    dto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(attachment);
                    dto.Attachment.Url = _attachmentManager.GetUrl(attachment);
                }
            }

            if (dto.Questions != null)
            {
                foreach (var qDto in dto.Questions)
                {
                    var question = entity.Questions.FirstOrDefault(x => x.Id == qDto.Id);
                    if (question != null && question.AttachmentId.HasValue)
                    {
                        var qAttachment = await _attachmentRepository.GetAsync(question.AttachmentId.Value);
                        if (qAttachment != null)
                        {
                            qDto.Attachment = ObjectMapper.Map<LiteAttachmentDto>(qAttachment);
                            qDto.Attachment.Url = _attachmentManager.GetUrl(qAttachment);
                        }
                    }
                    qDto.Category = entity.Subject?.Name?.FirstOrDefault()?.Name ?? "General";
                }
            }

            return dto;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto input)
        {
            var question = ObjectMapper.Map<Question>(input);
            await _questionRepository.InsertAsync(question);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public Task<List<QuestionDto>> AddQuestionsAsync(List<CreateQuestionDto> input)
        {
            throw new NotImplementedException();
        }

        public Task<QuestionDto> UpdateQuestionAsync(QuestionDto input)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteQuestionAsync(EntityDto<Guid> input)
        {
            await _questionRepository.DeleteAsync(input.Id);
        }
    }
}

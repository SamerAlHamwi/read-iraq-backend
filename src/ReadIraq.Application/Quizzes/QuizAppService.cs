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
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Teachers;

namespace ReadIraq.Quizzes
{
    [AbpAuthorize]
    public class QuizAppService : ReadIraqAsyncCrudAppService<Quiz, QuizDto, Guid, QuizDto, PagedQuizResultRequestDto, CreateQuizDto, QuizDto>, IQuizAppService
    {
        private readonly IRepository<Question, Guid> _questionRepository;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Attachment, long> _attachmentRepository;
        private readonly IRepository<Subject, Guid> _subjectRepository;
        private readonly IRepository<LessonSession, Guid> _lessonSessionRepository;
        private readonly IRepository<TeacherProfile, Guid> _teacherProfileRepository;

        public QuizAppService(
            IRepository<Quiz, Guid> repository,
            IRepository<Question, Guid> questionRepository,
            IAttachmentManager attachmentManager,
            IRepository<Attachment, long> attachmentRepository,
            IRepository<Subject, Guid> subjectRepository,
            IRepository<LessonSession, Guid> lessonSessionRepository,
            IRepository<TeacherProfile, Guid> teacherProfileRepository)
            : base(repository)
        {
            _questionRepository = questionRepository;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _subjectRepository = subjectRepository;
            _lessonSessionRepository = lessonSessionRepository;
            _teacherProfileRepository = teacherProfileRepository;
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

        private async Task ValidateQuizInput(Guid? subjectId, Guid? sessionId, Guid? teacherId)
        {
            if (subjectId.HasValue && !await _subjectRepository.GetAll().AnyAsync(x => x.Id == subjectId.Value))
            {
                throw new Abp.UI.UserFriendlyException(L("SubjectNotFound"));
            }

            if (sessionId.HasValue && !await _lessonSessionRepository.GetAll().AnyAsync(x => x.Id == sessionId.Value))
            {
                throw new Abp.UI.UserFriendlyException(L("SessionNotFound"));
            }

            if (teacherId.HasValue && !await _teacherProfileRepository.GetAll().AnyAsync(x => x.Id == teacherId.Value))
            {
                throw new Abp.UI.UserFriendlyException(L("TeacherNotFound"));
            }
        }

        private async Task ValidateQuestionInput(Guid quizId)
        {
            if (!await Repository.GetAll().AnyAsync(x => x.Id == quizId))
            {
                throw new Abp.UI.UserFriendlyException(L("QuizNotFound"));
            }
        }

        public override async Task<QuizDto> CreateAsync(CreateQuizDto input)
        {
            CheckCreatePermission();
            await ValidateQuizInput(input.SubjectId, input.SessionId, input.TeacherId);
            return await base.CreateAsync(input);
        }

        public override async Task<QuizDto> UpdateAsync(QuizDto input)
        {
            CheckUpdatePermission();
            await ValidateQuizInput(input.SubjectId, input.SessionId, input.TeacherId);
            return await base.UpdateAsync(input);
        }

        protected override async Task<Quiz> GetEntityByIdAsync(Guid id)
        {
            var entity = await Repository.FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("QuizNotFound"));
            }
            return entity;
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
                throw new Abp.UI.UserFriendlyException(L("QuizNotFoundForThisLesson"));
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
                throw new Abp.UI.UserFriendlyException(L("QuizNotFound"));
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
                var attachment = await _attachmentRepository.FirstOrDefaultAsync(entity.AttachmentId.Value);
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
                        var qAttachment = await _attachmentRepository.FirstOrDefaultAsync(question.AttachmentId.Value);
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
            await ValidateQuestionInput(input.QuizId);
            var question = ObjectMapper.Map<Question>(input);
            if (question.Id == Guid.Empty)
            {
                question.Id = Guid.NewGuid();
            }
            if (input.Type == 0)
            {
                question.Type = ReadIraq.Enums.Enum.QuestionType.MCQ;
            }
            await _questionRepository.InsertAsync(question);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public async Task<List<QuestionDto>> AddQuestionsAsync(List<CreateQuestionDto> input)
        {
            if (input == null || !input.Any()) return new List<QuestionDto>();

            foreach (var q in input)
            {
                await ValidateQuestionInput(q.QuizId);
            }

            var questions = new List<Question>();
            foreach (var qDto in input)
            {
                var question = ObjectMapper.Map<Question>(qDto);
                if (question.Id == Guid.Empty)
                {
                    question.Id = Guid.NewGuid();
                }

                if (qDto.Type == 0)
                {
                    question.Type = ReadIraq.Enums.Enum.QuestionType.MCQ;
                }

                await _questionRepository.InsertAsync(question);
                questions.Add(question);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<List<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> UpdateQuestionAsync(QuestionDto input)
        {
            await ValidateQuestionInput(input.QuizId);
            var question = await _questionRepository.FirstOrDefaultAsync(input.Id);
            if (question == null)
            {
                throw new UserFriendlyException(L("QuestionNotFound"));
            }

            ObjectMapper.Map(input, question);
            await _questionRepository.UpdateAsync(question);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<QuestionDto>(question);
        }

        public async Task DeleteQuestionAsync(EntityDto<Guid> input)
        {
            await _questionRepository.DeleteAsync(input.Id);
        }
    }
}

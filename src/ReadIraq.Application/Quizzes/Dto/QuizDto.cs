using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Quizzes;
using System;
using System.Collections.Generic;

namespace ReadIraq.Quizzes.Dto
{
    [AutoMapFrom(typeof(Quiz))]
    public class QuizDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? TeacherId { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalMarks { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    [AutoMapFrom(typeof(Question))]
    public class QuestionDto : EntityDto<Guid>
    {
        public Guid QuizId { get; set; }
        public byte Type { get; set; }
        public string Text { get; set; }
        public string Options { get; set; }
        public string CorrectAnswer { get; set; }
        public int Marks { get; set; }
    }
}

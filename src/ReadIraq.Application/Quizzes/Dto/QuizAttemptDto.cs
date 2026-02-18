using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Quizzes;
using System;

namespace ReadIraq.Quizzes.Dto
{
    [AutoMapFrom(typeof(QuizAttempt))]
    public class QuizAttemptDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid QuizId { get; set; }
        public string Answers { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime TakenAt { get; set; }
    }

    [AutoMapTo(typeof(QuizAttempt))]
    public class CreateQuizAttemptDto
    {
        public long UserId { get; set; }
        public Guid QuizId { get; set; }
        public string Answers { get; set; }
        public int Score { get; set; }
        public bool Passed { get; set; }
        public DateTime TakenAt { get; set; }
    }
}

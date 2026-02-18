using Abp.AutoMapper;
using ReadIraq.Domain.Quizzes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Quizzes.Dto
{
    [AutoMapTo(typeof(Quiz))]
    public class CreateQuizDto
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? TeacherId { get; set; }
        public int DurationSeconds { get; set; }
        public int TotalMarks { get; set; }
    }

    [AutoMapTo(typeof(Question))]
    public class CreateQuestionDto
    {
        [Required]
        public Guid QuizId { get; set; }
        public byte Type { get; set; }
        [Required]
        public string Text { get; set; }
        public string Options { get; set; }
        public string CorrectAnswer { get; set; }
        public int Marks { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Quizzes
{
    [Table("Questions")]
    public class Question : FullAuditedEntity<Guid>
    {
        public Guid QuizId { get; set; }
        [ForeignKey(nameof(QuizId))]
        public virtual Quiz Quiz { get; set; }

        public QuestionType Type { get; set; }
        public string Text { get; set; }

        /// <summary>
        /// Options for MCQ, stored as JSON string.
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Correct answer(s), stored as JSON string.
        /// </summary>
        public string CorrectAnswer { get; set; }

        public int Marks { get; set; }
    }
}

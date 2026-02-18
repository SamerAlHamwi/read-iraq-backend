using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Quizzes
{
    [Table("QuizAttempts")]
    public class QuizAttempt : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid QuizId { get; set; }
        [ForeignKey(nameof(QuizId))]
        public virtual Quiz Quiz { get; set; }

        /// <summary>
        /// User answers, stored as JSON string.
        /// </summary>
        public string Answers { get; set; }

        public int Score { get; set; }
        public bool Passed { get; set; }

        public DateTime TakenAt { get; set; }
    }
}

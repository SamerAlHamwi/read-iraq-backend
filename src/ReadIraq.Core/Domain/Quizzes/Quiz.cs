using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.LessonSessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Quizzes
{
    [Table("Quizzes")]
    public class Quiz : FullAuditedEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public Guid? SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public Guid? SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public virtual LessonSession Session { get; set; }

        public Guid? TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public virtual TeacherProfile Teacher { get; set; }

        public int DurationSeconds { get; set; }
        public int TotalMarks { get; set; }

        public long? AttachmentId { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public Quiz()
        {
            Questions = new HashSet<Question>();
        }
    }
}

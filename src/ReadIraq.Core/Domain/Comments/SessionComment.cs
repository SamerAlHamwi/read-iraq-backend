using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.LessonSessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Comments
{
    [Table("SessionComments")]
    public class SessionComment : FullAuditedEntity<Guid>
    {
        public Guid LessonSessionId { get; set; }

        [ForeignKey(nameof(LessonSessionId))]
        public virtual LessonSession LessonSession { get; set; }

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        public string Text { get; set; }

        public Guid? ParentCommentId { get; set; }

        [ForeignKey(nameof(ParentCommentId))]
        public virtual SessionComment ParentComment { get; set; }

        public virtual ICollection<SessionComment> Replies { get; set; }

        public bool IsByTeacher { get; set; }

        public SessionComment()
        {
            Replies = new HashSet<SessionComment>();
        }
    }
}

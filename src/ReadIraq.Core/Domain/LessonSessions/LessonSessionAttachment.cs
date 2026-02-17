using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Attachments;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace ReadIraq.Domain.LessonSessions
{
    [Table("LessonSessionAttachments")]
    public class LessonSessionAttachment : ISoftDelete
    {
        public Guid LessonSessionId { get; set; }

        [ForeignKey(nameof(LessonSessionId))]
        public virtual LessonSession LessonSession { get; set; }

        public long AttachmentId { get; set; }

        [ForeignKey(nameof(AttachmentId))]
        public virtual Attachment Attachment { get; set; }

        public bool IsDeleted
        {
            get; set;
        }
    }
}

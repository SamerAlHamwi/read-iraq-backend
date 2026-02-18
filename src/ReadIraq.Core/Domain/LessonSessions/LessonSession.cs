using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.LessonSessions
{
    [Table("LessonSessions")]
    public class LessonSession : FullAuditedEntity<Guid>, IPassivable
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(2000)]
        public string ThumbnailUrl { get; set; }

        [StringLength(2000)]
        public string VideoUrl { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        /// <summary>
        /// Presenter/teacher profile id.
        /// </summary>
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public int DurationSeconds { get; set; }

        /// <summary>
        /// Ordering inside module/collection.
        /// </summary>
        public int Order { get; set; }

        public bool IsSavedByDefault { get; set; }

        public int ViewsCount { get; set; }

        public int LikesCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsFree { get; set; }

        public virtual ICollection<LessonSessionAttachment> Attachments { get; set; }

        public LessonSession()
        {
            Attachments = new HashSet<LessonSessionAttachment>();
            IsActive = true;
            IsFree = false;
        }
    }
}

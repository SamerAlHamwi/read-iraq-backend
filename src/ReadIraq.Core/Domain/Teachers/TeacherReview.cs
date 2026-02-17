using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherReviews")]
    public class TeacherReview : FullAuditedEntity<Guid>
    {
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string ReviewText { get; set; }
    }
}

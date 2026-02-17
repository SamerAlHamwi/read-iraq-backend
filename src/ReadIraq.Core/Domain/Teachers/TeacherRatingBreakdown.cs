using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherRatingBreakdowns")]
    public class TeacherRatingBreakdown : FullAuditedEntity<Guid>
    {
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public int Rating { get; set; } // e.g., 1, 2, 3, 4, 5

        public int Count { get; set; }
    }
}

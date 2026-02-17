using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherFeaturesMap")]
    public class TeacherFeatureMap : FullAuditedEntity<Guid>
    {
        public Guid TeacherProfileId { get; set; }

        [ForeignKey(nameof(TeacherProfileId))]
        public virtual TeacherProfile TeacherProfile { get; set; }

        public Guid TeacherFeatureId { get; set; }

        [ForeignKey(nameof(TeacherFeatureId))]
        public virtual TeacherFeature TeacherFeature { get; set; }
    }
}

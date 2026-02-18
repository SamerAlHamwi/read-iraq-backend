using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherProfiles")]
    public class TeacherProfile : FullAuditedEntity<Guid>, IPassivable
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Bio { get; set; }

        public string Specialization { get; set; }

        public string AvatarUrl { get; set; }

        [StringLength(7)]
        public string Color { get; set; }

        public int StudentsCount { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; }

        public int ReviewsCount { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<TeacherFeatureMap> Features { get; set; }
        public virtual ICollection<TeacherSubject> Subjects { get; set; }
        public virtual ICollection<TeacherRatingBreakdown> RatingBreakdowns { get; set; }

        public TeacherProfile()
        {
            Features = new HashSet<TeacherFeatureMap>();
            Subjects = new HashSet<TeacherSubject>();
            RatingBreakdowns = new HashSet<TeacherRatingBreakdown>();
            IsActive = true;
        }
    }
}

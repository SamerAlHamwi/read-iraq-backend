using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Teachers
{
    [Table("TeacherFeatures")]
    public class TeacherFeature : FullAuditedEntity<Guid>, IPassivable
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }
        public bool IsActive { get; set; }

        public TeacherFeature()
        {
            IsActive = true;
        }
    }
}

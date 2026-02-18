using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Grades
{
    [Table("Grades")]
    public class Grade : FullAuditedEntity<int>, IEntity<int>
    {
        public virtual ICollection<Translation> Name { get; set; }

        public int Priority { get; set; }

        public Guid GradeGroupId { get; set; }

        [ForeignKey(nameof(GradeGroupId))]
        public virtual GradeGroup GradeGroup { get; set; }

        public virtual ICollection<GradeSubject> GradeSubjects { get; set; }

        public bool IsActive { get; set; } = true;

        public Grade()
        {
            Name = new HashSet<Translation>();
            GradeSubjects = new HashSet<GradeSubject>();
        }
    }
}

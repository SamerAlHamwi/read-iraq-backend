using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Translations;
using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.Grades
{
    public class GradeGroup : FullAuditedEntity<Guid>
    {
        public GradeGroup()
        {
            Name = new HashSet<Translation>();
        }

        public virtual ICollection<Translation> Name { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Subjects
{
    [Table("Subjects")]
    public class Subject : FullAuditedEntity<Guid>
    {
        public virtual ICollection<Translation> Name { get; set; }

        public Subject()
        {
            Name = new HashSet<Translation>();
        }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Translations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Subjects
{
    [Table("Subjects")]
    public class Subject : FullAuditedEntity<Guid>
    {
        public virtual ICollection<Translation> Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public SubjectLevel Level { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }

        public Subject()
        {
            Name = new HashSet<Translation>();
            StudentsCount = 0;
            TeachersCount = 0;
        }
    }
}

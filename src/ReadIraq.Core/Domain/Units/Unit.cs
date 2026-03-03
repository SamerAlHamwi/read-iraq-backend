using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Subjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Units
{
    [Table("Units")]
    public class Unit : FullAuditedEntity<Guid>, IPassivable
    {
        public virtual ICollection<Translation> Name { get; set; }

        public string Description { get; set; }

        public Guid SubjectId { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public int Order { get; set; }

        public bool IsActive { get; set; }

        public Unit()
        {
            Name = new HashSet<Translation>();
            IsActive = true;
        }
    }
}

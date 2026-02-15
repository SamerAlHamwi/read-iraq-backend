using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Grades
{
    public class Grade : FullAuditedEntity
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public int Priority { get; set; }
    }
}

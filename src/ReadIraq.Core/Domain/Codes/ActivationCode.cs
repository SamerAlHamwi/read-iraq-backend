using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Codes
{
    [Table("ActivationCodes")]
    public class ActivationCode : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        public Guid? SubjectId { get; set; }
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; }

        public Guid? TeacherId { get; set; }
        [ForeignKey(nameof(TeacherId))]
        public virtual TeacherProfile Teacher { get; set; }

        public int? GradeId { get; set; }
        [ForeignKey(nameof(GradeId))]
        public virtual Grade Grade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public DateTime? UsedDate { get; set; }

        public bool IsUsed => UserId.HasValue;

        public ActivationCode()
        {
            Id = Guid.NewGuid();
        }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Audit
{
    [Table("ActivityLogs")]
    public class ActivityLog : CreationAuditedEntity<Guid>
    {
        public long ActorId { get; set; }
        [ForeignKey(nameof(ActorId))]
        public virtual User Actor { get; set; }

        public string ActionType { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }

        /// <summary>
        /// Metadata, stored as JSON string.
        /// </summary>
        public string Metadata { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Subscriptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Gifts
{
    [Table("Gifts")]
    public class Gift : FullAuditedEntity<Guid>
    {
        public long TargetUserId { get; set; }

        [ForeignKey(nameof(TargetUserId))]
        public virtual User TargetUser { get; set; }

        public Guid? PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public virtual SubscriptionPlan Plan { get; set; }

        public string Note { get; set; }

        public long AdminUserId { get; set; }

        [ForeignKey(nameof(AdminUserId))]
        public virtual User AdminUser { get; set; }
    }
}

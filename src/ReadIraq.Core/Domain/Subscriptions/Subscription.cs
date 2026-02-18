using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Subscriptions
{
    [Table("Subscriptions")]
    public class Subscription : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public Guid PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public virtual SubscriptionPlan Plan { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; }

        public bool AutoRenew { get; set; }

        public int PointsAmount { get; set; }
    }
}

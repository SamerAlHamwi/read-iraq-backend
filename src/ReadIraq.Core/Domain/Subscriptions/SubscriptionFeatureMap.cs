using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Subscriptions
{
    [Table("SubscriptionFeatureMaps")]
    public class SubscriptionFeatureMap : FullAuditedEntity<Guid>
    {
        public Guid PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]
        public virtual SubscriptionPlan Plan { get; set; }

        public Guid FeatureId { get; set; }

        [ForeignKey(nameof(FeatureId))]
        public virtual SubscriptionFeature Feature { get; set; }
    }
}

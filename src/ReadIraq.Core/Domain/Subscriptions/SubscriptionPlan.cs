using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Subscriptions
{
    [Table("SubscriptionPlans")]
    public class SubscriptionPlan : FullAuditedEntity<Guid>
    {
        public string Name { get; set; }

        public int DurationDays { get; set; }

        public bool IsFeatured { get; set; }

        public decimal Price { get; set; }

        public decimal PriceBeforeDiscount { get; set; }

        public string DiscountText { get; set; }

        public string Currency { get; set; }

        public virtual ICollection<SubscriptionFeatureMap> Features { get; set; }

        public SubscriptionPlan()
        {
            Features = new HashSet<SubscriptionFeatureMap>();
        }
    }
}

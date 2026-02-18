using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Subscriptions
{
    [Table("SubscriptionFeatures")]
    public class SubscriptionFeature : FullAuditedEntity<Guid>
    {
        public string Name { get; set; }
    }
}

using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapTo(typeof(Subscription))]
    public class CreateSubscriptionDto
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid PlanId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; }

        public bool AutoRenew { get; set; }

        public int PointsAmount { get; set; }
    }
}

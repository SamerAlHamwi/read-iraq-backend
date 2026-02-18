using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapFrom(typeof(Subscription))]
    public class SubscriptionDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid PlanId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool AutoRenew { get; set; }
        public int PointsAmount { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapFrom(typeof(SubscriptionFeatureMap))]
    public class SubscriptionFeatureMapDto : EntityDto<Guid>
    {
        public Guid PlanId { get; set; }
        public Guid FeatureId { get; set; }
        public SubscriptionFeatureDto Feature { get; set; }
    }
}

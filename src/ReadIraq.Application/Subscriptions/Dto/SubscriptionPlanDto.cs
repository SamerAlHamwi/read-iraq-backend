using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System;
using System.Collections.Generic;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapFrom(typeof(SubscriptionPlan))]
    public class SubscriptionPlanDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public int DurationDays { get; set; }
        public bool IsFeatured { get; set; }
        public decimal Price { get; set; }
        public decimal PriceBeforeDiscount { get; set; }
        public string DiscountText { get; set; }
        public string Currency { get; set; }
        public List<SubscriptionFeatureMapDto> Features { get; set; }
    }
}

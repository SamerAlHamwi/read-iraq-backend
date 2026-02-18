using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapFrom(typeof(SubscriptionFeature))]
    public class SubscriptionFeatureDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}

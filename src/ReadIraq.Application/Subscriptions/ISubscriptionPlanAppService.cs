using Abp.Application.Services;
using ReadIraq.Subscriptions.Dto;
using System;

namespace ReadIraq.Subscriptions
{
    public interface ISubscriptionPlanAppService : IAsyncCrudAppService<SubscriptionPlanDto, Guid, PagedSubscriptionPlanResultRequestDto, CreateSubscriptionPlanDto, UpdateSubscriptionPlanDto>
    {
    }
}

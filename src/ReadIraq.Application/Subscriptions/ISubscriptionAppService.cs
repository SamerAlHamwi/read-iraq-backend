using Abp.Application.Services;
using ReadIraq.Subscriptions.Dto;
using System;

namespace ReadIraq.Subscriptions
{
    public interface ISubscriptionAppService : IAsyncCrudAppService<SubscriptionDto, Guid, PagedSubscriptionResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>
    {
    }
}

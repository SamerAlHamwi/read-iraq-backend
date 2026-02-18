using Abp.Application.Services;
using ReadIraq.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Subscriptions
{
    public interface ISubscriptionAppService : IAsyncCrudAppService<SubscriptionDto, Guid, PagedSubscriptionResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>
    {
        Task SubscribeAsync(Guid planId);
        Task<List<SubscriptionDto>> GetUserSubscriptionsAsync(long userId);
    }
}

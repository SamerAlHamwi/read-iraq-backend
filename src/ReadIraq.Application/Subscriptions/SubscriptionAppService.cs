using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Subscriptions.Dto;
using System;
using System.Linq;

namespace ReadIraq.Subscriptions
{
    [AbpAuthorize]
    public class SubscriptionAppService : ReadIraqAsyncCrudAppService<Subscription, SubscriptionDto, Guid, SubscriptionDto, PagedSubscriptionResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>, ISubscriptionAppService
    {
        public SubscriptionAppService(IRepository<Subscription, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<Subscription> CreateFilteredQuery(PagedSubscriptionResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.PlanId.HasValue, x => x.PlanId == input.PlanId.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }
    }
}

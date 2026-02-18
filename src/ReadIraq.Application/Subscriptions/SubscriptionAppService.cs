using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Subscriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Subscriptions
{
    [AbpAuthorize]
    public class SubscriptionAppService : ReadIraqAsyncCrudAppService<Subscription, SubscriptionDto, Guid, SubscriptionDto, PagedSubscriptionResultRequestDto, CreateSubscriptionDto, UpdateSubscriptionDto>, ISubscriptionAppService
    {
        private readonly IRepository<SubscriptionPlan, Guid> _planRepository;

        public SubscriptionAppService(
            IRepository<Subscription, Guid> repository,
            IRepository<SubscriptionPlan, Guid> planRepository)
            : base(repository)
        {
            _planRepository = planRepository;
        }

        protected override IQueryable<Subscription> CreateFilteredQuery(PagedSubscriptionResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Plan)
                .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId.Value)
                .WhereIf(input.PlanId.HasValue, x => x.PlanId == input.PlanId.Value)
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive.Value);
        }

        public async Task SubscribeAsync(Guid planId)
        {
            var plan = await _planRepository.FirstOrDefaultAsync(planId);
            if (plan == null)
            {
                throw new Abp.UI.UserFriendlyException(L("SubscriptionPlanNotFound"));
            }

            var userId = AbpSession.GetUserId();

            var activeSubscriptions = await Repository.GetAllListAsync(x => x.UserId == userId && x.IsActive);
            foreach (var sub in activeSubscriptions)
            {
                sub.IsActive = false;
            }

            var subscription = new Subscription
            {
                UserId = userId,
                PlanId = planId,
                StartedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(plan.DurationDays),
                IsActive = true,
                AutoRenew = false
            };

            await Repository.InsertAsync(subscription);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<List<SubscriptionDto>> GetUserSubscriptionsAsync(long userId)
        {
            if (userId != AbpSession.GetUserId() && !PermissionChecker.IsGranted(PermissionNames.Pages_Users))
            {
                throw new Abp.UI.UserFriendlyException(L("InsufficientPermissions"));
            }

            var subscriptions = await Repository.GetAll()
                .Include(x => x.Plan)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreationTime)
                .ToListAsync();

            return subscriptions.Select(MapToEntityDto).ToList();
        }
    }
}

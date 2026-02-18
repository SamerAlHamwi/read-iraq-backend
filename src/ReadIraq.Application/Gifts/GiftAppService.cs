using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Gifts;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Gifts.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Gifts
{
    [AbpAuthorize(Authorization.PermissionNames.Pages_Users)] // Only admin (or user managers) can grant gifts
    public class GiftAppService : ReadIraqAppServiceBase, IGiftAppService
    {
        private readonly IRepository<Gift, Guid> _giftRepository;
        private readonly IRepository<Subscription, Guid> _subscriptionRepository;
        private readonly IRepository<SubscriptionPlan, Guid> _planRepository;
        private readonly IRepository<User, long> _userRepository;

        public GiftAppService(
            IRepository<Gift, Guid> giftRepository,
            IRepository<Subscription, Guid> subscriptionRepository,
            IRepository<SubscriptionPlan, Guid> planRepository,
            IRepository<User, long> userRepository)
        {
            _giftRepository = giftRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _userRepository = userRepository;
        }

        public async Task GrantGiftAsync(GrantGiftDto input)
        {
            var adminUserId = AbpSession.GetUserId();
            SubscriptionPlan plan = null;
            if (input.PlanId.HasValue)
            {
                plan = await _planRepository.GetAsync(input.PlanId.Value);
            }

            foreach (var targetUserId in input.UserIds)
            {
                // 1. Record the gift
                await _giftRepository.InsertAsync(new Gift
                {
                    TargetUserId = targetUserId,
                    PlanId = input.PlanId,
                    Note = input.Note,
                    AdminUserId = adminUserId
                });

                // 2. Grant the subscription if plan is provided
                if (plan != null)
                {
                    // Deactivate old active subscriptions
                    var activeSubscriptions = await _subscriptionRepository.GetAllListAsync(x => x.UserId == targetUserId && x.IsActive);
                    foreach (var sub in activeSubscriptions)
                    {
                        sub.IsActive = false;
                    }

                    await _subscriptionRepository.InsertAsync(new Subscription
                    {
                        UserId = targetUserId,
                        PlanId = plan.Id,
                        StartedAt = DateTime.Now,
                        ExpiresAt = DateTime.Now.AddDays(plan.DurationDays),
                        IsActive = true,
                        AutoRenew = false
                    });
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResultDto<GiftDto>> GetAllAsync(PagedAndSortedResultRequestDto input)
        {
            var query = _giftRepository.GetAll()
                .Include(x => x.TargetUser)
                .Include(x => x.AdminUser)
                .Include(x => x.Plan);

            var totalCount = await query.CountAsync();

            var gifts = await query
                .OrderByDescending(x => x.CreationTime)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<GiftDto>(
                totalCount,
                gifts.Select(x => new GiftDto
                {
                    Id = x.Id,
                    TargetUserId = x.TargetUserId,
                    TargetUserName = x.TargetUser?.Name,
                    PlanId = x.PlanId,
                    PlanName = x.Plan?.Name,
                    Note = x.Note,
                    AdminUserId = x.AdminUserId,
                    AdminUserName = x.AdminUser?.Name,
                    CreationTime = x.CreationTime
                }).ToList()
            );
        }
    }
}

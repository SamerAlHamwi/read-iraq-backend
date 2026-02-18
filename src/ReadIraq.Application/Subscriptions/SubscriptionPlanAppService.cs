using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Subscriptions.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Subscriptions
{
    [AbpAuthorize]
    public class SubscriptionPlanAppService : ReadIraqAsyncCrudAppService<SubscriptionPlan, SubscriptionPlanDto, Guid, SubscriptionPlanDto, PagedSubscriptionPlanResultRequestDto, CreateSubscriptionPlanDto, UpdateSubscriptionPlanDto>, ISubscriptionPlanAppService
    {
        public SubscriptionPlanAppService(IRepository<SubscriptionPlan, Guid> repository)
            : base(repository)
        {
        }

        protected override IQueryable<SubscriptionPlan> CreateFilteredQuery(PagedSubscriptionPlanResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Include(x => x.Features)
                    .ThenInclude(x => x.Feature)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword));
        }

        public override async Task<SubscriptionPlanDto> GetAsync(EntityDto<Guid> input)
        {
            var entity = await Repository.GetAll()
                .Include(x => x.Features)
                    .ThenInclude(x => x.Feature)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (entity == null)
            {
                throw new Abp.UI.UserFriendlyException(L("SubscriptionPlanNotFound"));
            }

            return MapToEntityDto(entity);
        }

        [AbpAuthorize(PermissionNames.Pages_Users)] // Admin only
        public override Task<SubscriptionPlanDto> CreateAsync(CreateSubscriptionPlanDto input)
        {
            return base.CreateAsync(input);
        }

        [AbpAuthorize(PermissionNames.Pages_Users)] // Admin only
        public override Task<SubscriptionPlanDto> UpdateAsync(UpdateSubscriptionPlanDto input)
        {
            return base.UpdateAsync(input);
        }

        [AbpAuthorize(PermissionNames.Pages_Users)] // Admin only
        public override Task DeleteAsync(EntityDto<Guid> input)
        {
            return base.DeleteAsync(input);
        }
    }
}

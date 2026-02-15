using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.PrivacyPolicyService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PrivacyPolicyService
{
    public class PrivacyPolicyAppService : ReadIraqAsyncCrudAppService<PrivacyPolicy, PrivacyPolicyDetailsDto, int, LitePrivacyPolicyDto, PagedPrivacyPolicyResultRequestDto,
         CreatePrivacyPolicyDto, UpdatePrivacyPolicyDto>, IPrivacyPolicyAppService
    {
        private readonly IPrivacyPolicyManager _privacyPolicyManager;
        public PrivacyPolicyAppService(IRepository<PrivacyPolicy, int> repository, IPrivacyPolicyManager privacyPolicyManager) : base(repository)
        {
            _privacyPolicyManager = privacyPolicyManager;
        }
        public override async Task<PrivacyPolicyDetailsDto> GetAsync(EntityDto<int> input)
        {
            var privacyPolicy = await _privacyPolicyManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PrivacyPolicy));
            return MapToEntityDto(privacyPolicy);
        }
        public override async Task<PagedResultDto<LitePrivacyPolicyDto>> GetAllAsync(PagedPrivacyPolicyResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            return result;
        }
        [AbpAuthorize(PermissionNames.PrivacyPolicy_FullControl)]
        public override async Task<PrivacyPolicyDetailsDto> CreateAsync(CreatePrivacyPolicyDto input)
        {
            //if (await _termManager.CheckIfAnyPolicyExist())
            //    throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.PrivacyPolicy));
            var Translation = ObjectMapper.Map<List<PrivacyPolicyTranslation>>(input.Translations);
            var privacyPolicy = ObjectMapper.Map<PrivacyPolicy>(input);
            privacyPolicy.IsActive = true;
            await Repository.InsertAsync(privacyPolicy);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<PrivacyPolicyDetailsDto>(privacyPolicy);

        }
        [AbpAuthorize(PermissionNames.PrivacyPolicy_FullControl)]
        public override async Task<PrivacyPolicyDetailsDto> UpdateAsync(UpdatePrivacyPolicyDto input)
        {
            var privacyPolicy = await _privacyPolicyManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PrivacyPolicy));
            privacyPolicy.Translations.Clear();
            MapToEntity(input, privacyPolicy);
            privacyPolicy.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(privacyPolicy);
            return MapToEntityDto(privacyPolicy);
        }
        [AbpAuthorize(PermissionNames.PrivacyPolicy_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            var privacyPolicy = await _privacyPolicyManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PrivacyPolicy));
            await Repository.HardDeleteAsync(privacyPolicy);
        }

        protected override IQueryable<PrivacyPolicy> CreateFilteredQuery(PagedPrivacyPolicyResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                var keyword = input.Keyword.ToLower();
                data = data.Where(x => x.Translations.Any(x => x.Title.ToLower().Contains(keyword)
                || x.Description.ToLower().Contains(keyword)));
            }
            data = data.Where(x => x.IsForMoney == input.IsForMoney);
            if (input.App.HasValue)
                data = data.Where(x => x.App == input.App.Value);
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);
            return data;

        }
        protected override IQueryable<PrivacyPolicy> ApplySorting(IQueryable<PrivacyPolicy> query, PagedPrivacyPolicyResultRequestDto input)
        {
            return query.OrderBy(r => r.App).ThenBy(x => x.OrderNo);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.PrivacyPolicy_FullControl)]
        public async Task<PrivacyPolicyDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            entity.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return MapToEntityDto(entity);
        }

        [HttpPut]
        [AbpAuthorize(PermissionNames.PrivacyPolicy_FullControl)]
        public async Task<PrivacyPolicyDetailsDto> MoveOrderNoAsync(MoveOrderNoDto input)
        {
            CheckUpdatePermission();

            var entity = await Repository.GetAsync(input.Id);
            if (input.IsUp)
            {
                if (entity.OrderNo == 1)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.IncompatibleValue, Tokens.PrivacyPolicy));
                }

                var prevRecord = Repository.GetAllList(x => x.App == entity.App && x.OrderNo < entity.OrderNo).OrderByDescending(x => x.OrderNo).FirstOrDefault();
                prevRecord.OrderNo++;
                prevRecord.LastModificationTime = DateTime.UtcNow;
                await Repository.UpdateAsync(prevRecord);

                entity.OrderNo--;
            }
            else
            {
                var maxOrderNo = Repository.GetAllList(x => x.App == entity.App).Max(x => x.OrderNo);
                if (entity.OrderNo == maxOrderNo)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.IncompatibleValue, Tokens.PrivacyPolicy));
                }

                var nextRecord = Repository.GetAllList(x => x.App == entity.App && x.OrderNo > entity.OrderNo).OrderBy(x => x.OrderNo).FirstOrDefault();
                nextRecord.OrderNo--;
                nextRecord.LastModificationTime = DateTime.UtcNow;
                await Repository.UpdateAsync(nextRecord);

                entity.OrderNo++;
            }

            entity.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return MapToEntityDto(entity);
        }
    }
}

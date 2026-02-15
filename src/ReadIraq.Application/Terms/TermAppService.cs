using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Terms;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.TermService.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.TermService
{
    public class TermAppService : ReadIraqAsyncCrudAppService<Term, TermDetailsDto, int, LiteTermDto, PagedTermResultRequestDto,
         CreateTermDto, UpdateTermDto>, ITermAppService
    {
        private readonly ITermManager _termManager;
        public TermAppService(IRepository<Term, int> repository, ITermManager termManager) : base(repository)
        {
            _termManager = termManager;
        }
        public override async Task<TermDetailsDto> GetAsync(EntityDto<int> input)
        {
            var privacyPolicy = await _termManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Term));
            return MapToEntityDto(privacyPolicy);
        }
        public override async Task<PagedResultDto<LiteTermDto>> GetAllAsync(PagedTermResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);
            return result;
        }
        [AbpAuthorize(PermissionNames.Terms_FullControl)]
        public override async Task<TermDetailsDto> CreateAsync(CreateTermDto input)
        {
            //if (await _termManager.CheckIfAnyPolicyExist())
            //    throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.Term));
            var Translation = ObjectMapper.Map<List<TermTranslation>>(input.Translations);
            var privacyPolicy = ObjectMapper.Map<Term>(input);
            privacyPolicy.IsActive = true;
            await Repository.InsertAsync(privacyPolicy);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<TermDetailsDto>(privacyPolicy);

        }
        [AbpAuthorize(PermissionNames.Terms_FullControl)]
        public override async Task<TermDetailsDto> UpdateAsync(UpdateTermDto input)
        {
            var privacyPolicy = await _termManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Term));
            privacyPolicy.Translations.Clear();
            MapToEntity(input, privacyPolicy);
            privacyPolicy.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(privacyPolicy);
            return MapToEntityDto(privacyPolicy);
        }
        [AbpAuthorize(PermissionNames.Terms_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            var privacyPolicy = await _termManager.GetEntityByIdAsync(input.Id);
            if (privacyPolicy is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Term));
            await Repository.HardDeleteAsync(privacyPolicy);
        }

        protected override IQueryable<Term> CreateFilteredQuery(PagedTermResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                var keyword = input.Keyword.ToLower();
                data = data.Where(x => x.Translations.Any(x => x.Title.ToLower().Contains(keyword)
                || x.Description.ToLower().Contains(keyword)));
            }
            if (input.App.HasValue)
                data = data.Where(x => x.App == input.App.Value || x.App == AppType.Both);
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);
            return data;

        }
        protected override IQueryable<Term> ApplySorting(IQueryable<Term> query, PagedTermResultRequestDto input)
        {
            return query.OrderBy(r => r.Id);
        }
        [HttpPut]
        [AbpAuthorize(PermissionNames.Terms_FullControl)]
        public async Task<TermDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var entity = await Repository.GetAsync(input.Id);
            entity.IsActive = input.IsActive;
            entity.LastModificationTime = DateTime.UtcNow;
            await Repository.UpdateAsync(entity);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return MapToEntityDto(entity);
        }
    }
}

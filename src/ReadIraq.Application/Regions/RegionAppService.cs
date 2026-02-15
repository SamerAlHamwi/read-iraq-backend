using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicSystem.Regions
{
    public class RegionAppService : ReadIraqAsyncCrudAppService<Region, RegionDetailsDto, int, LiteRegionDto,
        PagedRegionResultRequestDto, CreateRegionDto, UpdateRegionDto>,
        IRegionAppService
    {
        private readonly IRegionManager _regionManager;
        private readonly ICountryManager _countryManager;
        private readonly IRepository<RegionTranslation> _regionTranslationRepository;
        private readonly IRepository<Region> _regionRepository;



        /// <summary>
        /// Region AppService
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="regionManager"></param>
        /// <param name="countryManager"></param>
        /// <param name="regionTranslationRepository"></param>

        public RegionAppService(IRepository<Region> repository, RegionManager regionManager,
            ICountryManager countryManager,
            IRepository<RegionTranslation> regionTranslationRepository,
            IRepository<Region> regionRepository)
            : base(repository)
        {

            _regionManager = regionManager;
            _countryManager = countryManager;
            _regionTranslationRepository = regionTranslationRepository;
            _regionRepository = regionRepository;
        }
        /// <summary>
        /// Get Region Details ById
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<RegionDetailsDto> GetAsync(EntityDto<int> input)
        {
            var region = await _regionManager.GetEntityByIdAsync(input.Id);
            if (region is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Region));
            return MapToEntityDto(region);
        }
        /// <summary>
        /// Get All Regions Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteRegionDto>> GetAllAsync(PagedRegionResultRequestDto input)
        {

            return await base.GetAllAsync(input);
        }
        /// <summary>
        /// Add New Region Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Region_FullControl)]
        public override async Task<RegionDetailsDto> CreateAsync(CreateRegionDto input)
        {
            CheckCreatePermission();
            var Translation = ObjectMapper.Map<List<RegionTranslation>>(input.Translations);
            if (await _regionManager.CheckIfRegionIsExist(Translation))
                throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.Region));
            var region = ObjectMapper.Map<Region>(input);
            region.IsActive = true;
            region.CreationTime = DateTime.UtcNow;
            await _regionRepository.InsertAsync(region);
            return MapToEntityDto(region);
        }
        /// <summary>
        /// Update Region Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Region_FullControl)]
        public override async Task<RegionDetailsDto> UpdateAsync(UpdateRegionDto input)
        {
            CheckUpdatePermission();
            var region = await _regionManager.GetEntityByIdAsync(input.Id);
            if (region is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Region));
            await _regionTranslationRepository.GetAll().Where(x => x.CoreId == input.Id).
                          ExecuteUpdateAsync(se => se.SetProperty(x => x.IsDeleted, true));
            MapToEntity(input, region);
            region.LastModificationTime = DateTime.UtcNow;
            await _regionRepository.UpdateAsync(region);
            return MapToEntityDto(region);
        }

        /// <summary>
        /// Delete Region Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Region_FullControl)]

        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();
            var region = await _regionManager.GetEntityByIdAsync(input.Id);
            if (region is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Region));
            foreach (var translation in region.Translations.ToList())
            {
                await _regionTranslationRepository.DeleteAsync(translation);
                region.Translations.Remove(translation);
            }
            await _regionRepository.DeleteAsync(region);
        }

        /// <summary>
        /// Filter For A Group Of Regions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Region> CreateFilteredQuery(PagedRegionResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations).AsNoTracking();
            data = data.Include(x => x.City).ThenInclude(x => x.Translations).AsNoTracking();
            data = data.Include(x => x.City).ThenInclude(x => x.Country).ThenInclude(c => c.Translations).AsNoTracking();
            if (!input.Keyword.IsNullOrEmpty())
                data = data.Where(x => x.Translations.Where(x => x.Name.Contains(input.Keyword)).Any());
            if (input.CityId.HasValue)
                data = data.Where(x => x.CityId == input.CityId);
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);
            return data;
        }

        /// <summary>
        /// Sorting Filtered Regions
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Region> ApplySorting(IQueryable<Region> query, PagedRegionResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime);
        }

        /// <summary>
        /// Switch Activation Of A Region
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [AbpAuthorize(PermissionNames.Region_FullControl)]
        public async Task<RegionDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var region = await _regionManager.GetLiteEntityByIdAsync(input.Id);
            if (region is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Region));
            region.IsActive = input.IsActive;
            await _regionRepository.UpdateAsync(region);
            return MapToEntityDto(region);
        }

    }
}


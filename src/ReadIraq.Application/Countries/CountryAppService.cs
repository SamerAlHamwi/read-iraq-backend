using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.Countries.Dto;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Countries;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Countries
{
    /// <summary>
    /// 
    /// </summary>
    public class CountryAppService : ReadIraqAsyncCrudAppService<Country, CountryDetailsDto, int, CountryDto,
        PagedCountryResultRequestDto, CreateCountryDto, UpdateCountryDto>,
        ICountryAppService
    {
        private readonly ICountryManager _countryManager;
        private readonly IRepository<CountryTranslation> _countryTranslationRepository;
        private readonly IRepository<Country> _countryRepository;
        /// <summary>
        /// Countries AppService
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="countryManager"></param>
        /// <param name="countryTranslationRepository"></param>
        public CountryAppService(IRepository<Country> repository,
            ICountryManager countryManager,
            IRepository<CountryTranslation> countryTranslationRepository,
            IRepository<Country> countryRepository)
         : base(repository)
        {
            _countryManager = countryManager;
            _countryTranslationRepository = countryTranslationRepository;
            _countryRepository = countryRepository;
        }
        /// <summary>
        /// Get Country Details ById
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<CountryDetailsDto> GetAsync(EntityDto<int> input)
        {
            var country = await _countryManager.GetEntityByIdAsync(input.Id);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            return MapToEntityDto(country);
        }
        /// <summary>
        /// Get All Countries Details 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<CountryDto>> GetAllAsync(PagedCountryResultRequestDto input)
        {

            return await base.GetAllAsync(input);
        }
        /// <summary>
        /// Add New Country  Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Country_FullControl)]
        public override async Task<CountryDetailsDto> CreateAsync(CreateCountryDto input)
        {
            CheckCreatePermission();
            var Translation = ObjectMapper.Map<List<CountryTranslation>>(input.Translations);
            if (await _countryManager.CheckIfCountryExist(Translation))
                throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.Country));
            var country = ObjectMapper.Map<Country>(input);
            country.IsActive = true;
            await _countryRepository.InsertAsync(country);
            return MapToEntityDto(country);
        }
        /// <summary>
        /// Update Country Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Country_FullControl)]
        public override async Task<CountryDetailsDto> UpdateAsync(UpdateCountryDto input)
        {

            CheckUpdatePermission();
            var country = await _countryManager.GetEntityByIdAsync(input.Id);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            await _countryTranslationRepository.GetAll().Where(x => x.CoreId == input.Id).
                ExecuteUpdateAsync(se => se.SetProperty(x => x.IsDeleted, true));
            MapToEntity(input, country);
            country.LastModificationTime = DateTime.UtcNow;
            await _countryRepository.UpdateAsync(country);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new CountryDetailsDto();

        }
        /// <summary>
        /// Delete Country Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.Country_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();
            var country = await _countryManager.GetEntityByIdAsync(input.Id);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            if (country.Cities.Count > 0)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectCantBeDelete, Tokens.City));
            }
            foreach (var translation in country.Translations.ToList())
            {
                await _countryTranslationRepository.DeleteAsync(translation);
                country.Translations.Remove(translation);
            }
            await _countryRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// Filter For A Group of Countries
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        protected override IQueryable<Country> CreateFilteredQuery(PagedCountryResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            if (!input.Keyword.IsNullOrEmpty())
                data = data.Where(x => x.Translations.Where(x => x.Name.Contains(input.Keyword)).Any() || x.DialCode.Contains(input.Keyword));
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);

            return data;
        }
        /// <summary>
        /// Sorting Filtered Countries
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<Country> ApplySorting(IQueryable<Country> query, PagedCountryResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime);
        }
        /// <summary>
        /// Switch Activation For A Country
        /// </summary>
        /// <param name="input"></param>
        ///// <returns></returns>
        [HttpPut]
        [AbpAuthorize(PermissionNames.Country_FullControl)]
        public async Task<CountryDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var country = await _countryManager.GetLiteEntityByIdAsync(input.Id);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            country.IsActive = input.IsActive;
            country.LastModificationTime = DateTime.UtcNow;
            await _countryRepository.UpdateAsync(country);
            return MapToEntityDto(country);

        }
    }
}

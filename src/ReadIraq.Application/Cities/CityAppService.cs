using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.Cities.Dto;
using ReadIraq.Countries;
using ReadIraq.CrudAppServiceBase;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Countries;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Cities
{

    /// <summary>
    /// 
    /// </summary>
    public class CityAppService : ReadIraqAsyncCrudAppService<City, CityDetailsDto, int, LiteCityDto,
        PagedCityResultRequestDto, CreateCityDto, UpdateCityDto>,
        ICityAppService
    {
        private readonly CityManager _cityManager;
        private readonly CountryManager _countryManager;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<CityTranslation> _cityTranslationRepository;
        private readonly IRepository<City> _cityRepository;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="cityManager"></param>
        /// <param name="countryManager"></param>
        /// <param name="countryRepository"></param>
        /// <param name="cityTranslationRepository"></param>
        /// <param name="cityRepository"></param>
        /// <param name="attachmentManager"></param>
        public CityAppService(IRepository<City> repository,
            CityManager cityManager,
            CountryManager countryManager,
            IRepository<Country> countryRepository,
            IRepository<CityTranslation> cityTranslationRepository
,
            IRepository<City> cityRepository)
         : base(repository)
        {
            _cityManager = cityManager;
            _countryManager = countryManager;
            _countryRepository = countryRepository;
            _cityTranslationRepository = cityTranslationRepository;
            _cityRepository = cityRepository;
        }

        /// <summary>
        /// Get City ByID
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<CityDetailsDto> GetAsync(EntityDto<int> input)
        {
            var city = await _cityManager.GetEntityByIdAsync(input.Id);

            var cityDetailsDto = MapToEntityDto(city);

            return cityDetailsDto;
        }
        /// <summary>
        /// Get All City
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        public override async Task<PagedResultDto<LiteCityDto>> GetAllAsync(PagedCityResultRequestDto input)
        {
            var result = await base.GetAllAsync(input);

            return result;
        }

        [AbpAuthorize(PermissionNames.City_FullControl)]
        /// <summary>
        /// Add New City
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<CityDetailsDto> CreateAsync(CreateCityDto input)
        {
            CheckCreatePermission();
            var country = await _countryManager.GetLiteEntityByIdAsync(input.CountryId);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            var Translation = ObjectMapper.Map<List<CityTranslation>>(input.Translations);
            if (await _cityManager.CheckIfCityIsExist(Translation))
                throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.City));
            var city = ObjectMapper.Map<City>(input);
            city.CreationTime = DateTime.UtcNow;
            city.IsActive = true;
            await Repository.InsertAsync(city);
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToEntityDto(city);

        }
        /// <summary>
        /// Update City Details
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.City_FullControl)]
        public override async Task<CityDetailsDto> UpdateAsync(UpdateCityDto input)
        {
            CheckUpdatePermission();
            var city = await _cityManager.GetEntityByIdAsync(input.Id);
            if (city is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.City));
            await _cityTranslationRepository.GetAll().Where(x => x.CoreId == input.Id).ExecuteDeleteAsync();
            var country = _countryRepository.GetAsync(input.CountryId);
            if (country is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.Country));
            MapToEntity(input, city);

            await _cityRepository.UpdateAsync(city);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return MapToEntityDto(city);
        }


        /// <summary>
        /// Delete A City 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(PermissionNames.City_FullControl)]
        public override async Task DeleteAsync(EntityDto<int> input)
        {
            CheckDeletePermission();
            var city = await _cityManager.GetEntityByIdAsync(input.Id);
            if (city is null)
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.City));

            foreach (var translation in city.Translations.ToList())
            {
                await _cityTranslationRepository.DeleteAsync(translation);
                city.Translations.Remove(translation);
            }

            await _cityRepository.DeleteAsync(input.Id);
        }

        /// <summary>
        /// Filter for  A Group of City
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<City> CreateFilteredQuery(PagedCityResultRequestDto input)
        {
            var data = base.CreateFilteredQuery(input);
            data = data.Include(x => x.Translations);
            data = data.Include(x => x.Country).ThenInclude(x => x.Translations);

            if (!input.Keyword.IsNullOrEmpty())
                data = data.Where(x => x.Translations.Where(x => x.Name.Contains(input.Keyword)).Any());
            if (input.CountryId.HasValue)
                data = data.Where(x => x.CountryId == input.CountryId);
            if (input.IsActive.HasValue)
                data = data.Where(x => x.IsActive == input.IsActive.Value);

            return data;
        }
        /// <summary>
        /// Sort Filtered Cities
        /// </summary>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override IQueryable<City> ApplySorting(IQueryable<City> query, PagedCityResultRequestDto input)
        {
            return query.OrderByDescending(r => r.CreationTime);
        }
        /// <summary>
        /// Switch Activation of A City
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [AbpAuthorize(PermissionNames.City_FullControl)]
        public async Task<CityDetailsDto> SwitchActivationAsync(SwitchActivationInputDto input)
        {
            CheckUpdatePermission();
            var city = await _cityManager.GetLiteEntityByIdAsync(input.Id);
            city.IsActive = input.IsActive;
            city.LastModificationTime = DateTime.UtcNow;
            await _cityRepository.UpdateAsync(city);
            return MapToEntityDto(city);
        }

    }
}

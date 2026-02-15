using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Cities.Dto;
using ReadIraq.Countries;
using ReadIraq.Domain.Countries;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Cities
{
    //city manager
    public class CityManager : DomainService, ICityManager
    {
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly ICountryManager _countryManager;
        private readonly IRepository<CityTranslation> _cityTranslationRepository;


        public CityManager(IRepository<City> cityRepository,
            IRepository<Country> countryRepository,
            ICountryManager countryManager,
            IRepository<CityTranslation> cityTranslationRepository)
        {
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _countryManager = countryManager;
            _cityTranslationRepository = cityTranslationRepository;
        }

        public async Task<bool> CheckIfCityIsExist(List<CityTranslation> Translations)
        {

            var cities = await _cityTranslationRepository.GetAll().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var city in cities)
                    if (city.Name == Translation.Name && city.Language == Translation.Language)
                        return true;
            }

            return false;
        }

        public async Task<int> GetCitiesCounts()
        {
            return await _cityRepository.GetAll().CountAsync();
        }

        public async Task<City> GetEntityByIdAsync(int id)
        {
            var entity = await _cityRepository.GetAll()
                .AsNoTracking()
                .Include(c => c.Translations)
                .Include(c => c.Country).ThenInclude(c => c.Translations)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(City), id);
            return entity;
        }
        public async Task<LiteCityDto> GetEntityDtoByIdAsync(int id)
        {
            return ObjectMapper.Map<LiteCityDto>(await GetEntityByIdAsync(id));
        }

        public async Task<City> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _cityRepository.GetAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(City), id);
            return entity;
        }

        public async Task<List<string>> GetAllCityNameForAutoComplete(string inputAutoComplete)
        {
            return await _cityTranslationRepository.GetAll().Where(x => x.Name.Contains(inputAutoComplete)).Select(x => x.Name).ToListAsync();
        }

        public async Task<bool> CheckIfCityIsExist(int cityId)
        {
            return await _cityRepository.GetAll().AnyAsync(x => x.Id == cityId);
        }

        public async Task<List<City>> CheckAndGetCitiesById(List<int> citesId)
        {
            List<City> cities = new List<City>();
            foreach (var cityId in citesId)
            {
                if (!await CheckIfCityIsExist(cityId))
                    throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.City + cityId.ToString()));
                var city = await GetLiteEntityByIdAsync(cityId);
                cities.Add(city);
            }
            return cities;
        }
    }
}

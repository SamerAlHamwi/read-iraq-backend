using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Countries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Countries
{
    public class CountryManager : DomainService, ICountryManager
    {
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<CountryTranslation> _countryTranslationRepository;

        public CountryManager(IRepository<Country> countryRepository,
            IRepository<CountryTranslation> countryTranslationRepository
           )
        {
            _countryRepository = countryRepository;
            _countryTranslationRepository = countryTranslationRepository;

        }


        public async Task<bool> CheckIfCountryExist(List<CountryTranslation> Translations)
        {
            var countries = await _countryTranslationRepository.GetAll().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var country in countries)
                    if (country.Name == Translation.Name && country.Language == Translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<Country> GetEntityByIdAsync(int id)
        {
            var entity = await _countryRepository.GetAll()
                .Include(c => c.Translations)
                .Include(c => c.Cities).ThenInclude(c => c.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Country), id);
            return entity;
        }

        public async Task<Country> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _countryRepository.GetAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Country), id);
            return entity;
        }



    }
}

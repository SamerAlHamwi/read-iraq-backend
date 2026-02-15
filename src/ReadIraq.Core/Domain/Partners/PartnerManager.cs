using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Partners
{
    public class PartnerManager : DomainService, IPartnerManager
    {
        private readonly IRepository<Partner> _partnerRepository;
        private readonly IRepository<Code> _codeRepository;
        private readonly ICityManager _cityManager;

        public PartnerManager(IRepository<Partner> partnerRepository, IRepository<Code> codeRepository, ICityManager cityManager)
        {
            _partnerRepository = partnerRepository;
            _codeRepository = codeRepository;
            _cityManager = cityManager;
        }

        public async Task<bool> CheckIfPartnerExist(string phoneNumber)
        {
            var partner = await _partnerRepository.GetAll().Where(x => x.PartnerPhoneNumber == phoneNumber).FirstOrDefaultAsync();
            if (partner != null)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> CheckIfPartnerExistForUpdate(string phoneNumber, int partnerId)
        {
            var partner = await _partnerRepository.GetAll().Where(x => x.PartnerPhoneNumber == phoneNumber && x.Id != partnerId).FirstOrDefaultAsync();
            if (partner != null)
            {
                return true;
            }
            return false;
        }
        public async Task<Partner> GetEntityByIdAsync(int id)
        {
            var entity = await _partnerRepository.GetAll().Include(x => x.CitiesPartner)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Partner), id);
            return entity;
        }
        public async Task<Partner> GetFullEntityByIdAsync(int id)
        {
            var entity = await _partnerRepository.GetAll().Include(x => x.CitiesPartner).ThenInclude(x => x.Translations).Include(x => x.CitiesPartner).ThenInclude(x => x.Country).ThenInclude(x => x.Translations).Include(x => x.Codes)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Partner), id);
            return entity;
        }

        public async Task<bool> CheckIfPartnerByIdExist(int id)
        {
            var partner = await _partnerRepository.GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (partner != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateCitiesForPartnerAsync(List<int> CitiesIds, Partner partner)
        {
            try
            {

                if (!CitiesIds.IsNullOrEmpty())
                {
                    var oldCities = partner.CitiesPartner.ToList();
                    var newCities = await _cityManager.CheckAndGetCitiesById(CitiesIds);

                    var cityToDelete = oldCities.Except(newCities).ToList();
                    foreach (var city in cityToDelete)
                    {
                        partner.CitiesPartner.Remove(city);
                    }
                    foreach (var city in newCities)
                    {
                        if (!partner.CitiesPartner.Contains(city))
                            partner.CitiesPartner.Add(city);
                    }
                }
                if (CitiesIds.IsNullOrEmpty() || CitiesIds.Count() == 0)
                    partner.CitiesPartner.Clear();

                await UnitOfWorkManager.Current.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { throw; }
        }

        public async Task<int> GetPartnersCount()
        {
            return await _partnerRepository.GetAll().AsNoTracking().Where(x => x.IsDeleted == false).CountAsync();
        }
    }
}

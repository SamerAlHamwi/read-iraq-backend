using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.Regions.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Regions
{
    public class RegionManager : DomainService, IRegionManager
    {
        private readonly IRepository<Region> _regionRepository;
        private readonly IRepository<RegionTranslation> _regionTranslationRepository;

        public RegionManager(IRepository<Region> regionRepository,
            IRepository<RegionTranslation> regionTranslationRepository)
        {
            _regionRepository = regionRepository;
            _regionTranslationRepository = regionTranslationRepository;
        }

        public async Task<Region> GetEntityByIdAsync(int id)
        {
            var entity = await _regionRepository.GetAll()
                 .Include(c => c.Translations)
                 .Include(c => c.City).ThenInclude(c => c.Translations)
                 .Include(c => c.City).ThenInclude(c => c.Country)
                 .ThenInclude(c => c.Translations)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(Region), id);
            return entity;
        }
        public async Task<LiteRegionDto> GetEntityDtoByIdAsync(int id)
        {
            var region = await GetEntityByIdAsync(id);
            return ObjectMapper.Map<LiteRegionDto>(region);
        }

        public async Task<Region> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _regionRepository.GetAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Region), id);
            return entity;
        }
        public async Task IsEntityExistAsync(int id)
        {
            var entity = await _regionRepository.GetAsync(id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(Region), id);
        }

        public async Task<bool> CheckIfRegionIsExist(List<RegionTranslation> Translations)
        {
            var regions = await _regionTranslationRepository.GetAll().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var region in regions)
                    if (region.Name == Translation.Name && region.Language == Translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<List<string>> GetAllRegionNameForAutoComplete(string inputAutoComplete)
        {
            return await _regionTranslationRepository.GetAll().Where(x => x.Name.Contains(inputAutoComplete)).Select(x => x.Name).ToListAsync();
        }
        public async Task<bool> CheckIfRegionIsExist(int cityId)
        {
            return await _regionRepository.GetAll().AnyAsync(x => x.Id == cityId);
        }


    }
}


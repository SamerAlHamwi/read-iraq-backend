using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.AttributesForSourceType;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeChoices
{
    public class AttributeChoiceManger : DomainService, IAttributeChoiceManger
    {
        private readonly IRepository<AttributeChoice> _attributeChoiceRepository;
        private readonly IRepository<AttributeForSourceType> _attributeForSourceTypeRepository;
        private readonly IRepository<AttributeChoiceTranslation> _attributeChoiceTranslationRepository;
        public AttributeChoiceManger(IRepository<AttributeChoice> attributeChoiceRepository,
            IRepository<AttributeForSourceType> attributeForSourceTypeRepository,
            IRepository<AttributeChoiceTranslation> attributeChoiceTranslationRepository)
        {
            _attributeChoiceRepository = attributeChoiceRepository;
            _attributeForSourceTypeRepository = attributeForSourceTypeRepository;
            _attributeChoiceTranslationRepository = attributeChoiceTranslationRepository;
        }

        public async Task<bool> CheckIfAttributeChoiceIsExist(List<AttributeChoiceTranslation> Translations)
        {
            var attributeChoiceTranslations = await _attributeChoiceTranslationRepository.GetAll().ToListAsync();
            foreach (var Translation in Translations)
            {
                foreach (var attributeChoice in attributeChoiceTranslations)
                    if (attributeChoice.Name == Translation.Name && attributeChoice.Language == Translation.Language)
                        return true;
            }
            return false;
        }

        public async Task<List<AttributeChoice>> GetAttributeChoicesByIdsAsync(List<int> Ids)
        {
            return await _attributeChoiceRepository.GetAll().Where(e => Ids.Contains(e.Id)).ToListAsync();
        }

        public async Task<AttributeChoice> GetEntityByIdAsync(int id)
        {
            var entity = await _attributeChoiceRepository.GetAll()
             .Include(c => c.Translations).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(AttributeChoice), id);
            return entity;
        }

        public async Task<AttributeChoice> GetLiteEntityByIdAsync(int id)
        {
            var entity = await _attributeChoiceRepository.GetAll().Include(x => x.Translations).Where(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                throw new EntityNotFoundException(typeof(AttributeChoice), id);
            return entity;
        }
        public async Task<List<AttributeChoice>> GetAttributeChoiceByPerantId(int id)
        {
            return await _attributeChoiceRepository.GetAll().Include(x => x.Translations).Where(x => x.AttributeChociceParentId == id).ToListAsync();
        }

        public async Task<List<AttributeChoiceTranslation>> GetAttributeChoiceTranslationByAttributeChoiceId(int attributeChoiceId)
        {
            return await _attributeChoiceTranslationRepository.GetAll().AsNoTracking().Where(x => x.CoreId == attributeChoiceId && x.IsDeleted == false).ToListAsync();
        }
        public async Task DeleteChildrenOfAttributeChoice(int attributeChoiceId)
        {
            var attributePerant = await GetAttributeChoiceByPerantId(attributeChoiceId);
            foreach (var attributeChoiceChild in attributePerant)
            {
                await SoftDeleteForEntityTranslation(attributeChoiceChild.Translations.ToList());
                await _attributeChoiceRepository.DeleteAsync(attributeChoiceChild);
            }
        }
        public async Task DeleteAllAttributeChoiceInAttribute(int attributeForSourceTypeId)
        {
            var attributeForSourceType = await _attributeChoiceRepository.GetAll().Include(x => x.Translations)
                .Where(x => x.AttributeForSourceTypeId == attributeForSourceTypeId && x.IsDeleted == false).ToListAsync();
            foreach (var attributeChoice in attributeForSourceType)
            {
                if (attributeChoice.IsAttributeChoiceParent)
                    await DeleteChildrenOfAttributeChoice(attributeChoice.Id);
                await SoftDeleteForEntityTranslation(attributeChoice.Translations.ToList());
                await _attributeChoiceRepository.DeleteAsync(attributeChoice);
            }
        }
        public async Task SoftDeleteForEntityTranslation(List<AttributeChoiceTranslation> translations)
        {
            foreach (var item in translations)
            {
                await _attributeChoiceTranslationRepository.DeleteAsync(item);
            }
        }

        public async Task<int> GetPointsToGiftToCompanyByAttributeChoices(List<int> choicesIds)
        {
            return await _attributeChoiceRepository.GetAll()
                   .AsNoTracking()
                   .Where(x => choicesIds.Contains(x.Id))
                   .SumAsync(x => x.PointsToGiftToCompany);
        }

        public async Task<int> GetPointsToBuyRequestByAttributeChoices(List<int> choicesIds)
        {
            return await _attributeChoiceRepository.GetAll()
                            .AsNoTracking()
                            .Where(x => choicesIds.Contains(x.Id))
                            .SumAsync(x => x.PointsToBuyRequest);
        }
    }
}

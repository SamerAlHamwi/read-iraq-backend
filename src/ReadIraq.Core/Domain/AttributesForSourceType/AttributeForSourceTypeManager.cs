using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using ReadIraq.Localization.SourceFiles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributesForSourceType
{
    public class AttributeForSourceTypeManager : DomainService, IAttributeForSourceTypeManager
    {
        private readonly IRepository<AttributeForSourceType> _attributeForSourceTypeRepository;
        private readonly IRepository<AttributeForSourceTypeTranslation> _attributeForSourceTypeTranslation;
        public AttributeForSourceTypeManager(IRepository<AttributeForSourceType> attributeForSourceTypeRepository, IRepository<AttributeForSourceTypeTranslation> attributeForSourceTypeTranslation)
        {
            _attributeForSourceTypeRepository = attributeForSourceTypeRepository;
            _attributeForSourceTypeTranslation = attributeForSourceTypeTranslation;
        }

        public async Task<bool> CheckAttributeForSourceTypeIsCorrect(List<CreateAttributeForSourceTypeValueDto> AttributeForSourceTypeValues)
        {
            var attributeForSourceType = await _attributeForSourceTypeRepository.GetAll().AsNoTrackingWithIdentityResolution().ToListAsync();

            foreach (var attriuteValue in AttributeForSourceTypeValues)
            {

                if (!attributeForSourceType.Any(x => x.Id == attriuteValue.AttributeForSourcTypeId))
                    throw new UserFriendlyException(Exceptions.ObjectWasNotFound + " " + Tokens.AttributeForSourceType + " " + attriuteValue.AttributeForSourcTypeId.ToString());

            }
            return true;
        }

        public async Task CheckIfAttribteChoiceHasPoints(int sourceTypeId)
        {
            if (await _attributeForSourceTypeRepository.GetAll()
                .Include(x => x.AttributeChoices)
                .Where(x => x.SourceTypes.Any(x => x.Id == sourceTypeId))
                .AnyAsync(x => x.AttributeChoices.Any(x => x.PointsToBuyRequest > 0 || x.PointsToGiftToCompany > 0)))
                throw new UserFriendlyException(Exceptions.YouCannotEdit, Tokens.AttributeChoiceHavePoints);
        }

        public async Task<AttributeForSourceType> GetEntityByIdAsync(int Id)
        {
            return await _attributeForSourceTypeRepository.GetAll().Include(x => x.Translations)
                .Include(x => x.SourceTypes).Include(x => x.AttributeChoices).ThenInclude(x => x.Translations).Where(x => x.Id == Id).FirstOrDefaultAsync() ??
                throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.AttributeForSourceType);

        }
        public async Task SoftDeleteForEntityTranslation(List<AttributeForSourceTypeTranslation> translations)
        {
            foreach (var item in translations)
            {
                await _attributeForSourceTypeTranslation.DeleteAsync(item);
            }
        }
    }
}

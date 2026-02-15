using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeChoices
{
    public interface IAttributeChoiceManger : IDomainService
    {
        Task<bool> CheckIfAttributeChoiceIsExist(List<AttributeChoiceTranslation> Translations);
        Task<AttributeChoice> GetEntityByIdAsync(int id);
        Task<AttributeChoice> GetLiteEntityByIdAsync(int id);
        Task DeleteChildrenOfAttributeChoice(int attributeChoiceId);
        Task<List<AttributeChoice>> GetAttributeChoicesByIdsAsync(List<int> Ids);
        Task<List<AttributeChoice>> GetAttributeChoiceByPerantId(int id);
        Task<List<AttributeChoiceTranslation>> GetAttributeChoiceTranslationByAttributeChoiceId(int attributeChoiceId);
        Task DeleteAllAttributeChoiceInAttribute(int AttributeForSourceTypeId);
        Task SoftDeleteForEntityTranslation(List<AttributeChoiceTranslation> translations);
        Task<int> GetPointsToGiftToCompanyByAttributeChoices(List<int> choicesIds);
        Task<int> GetPointsToBuyRequestByAttributeChoices(List<int> choicesIds);
    }
}
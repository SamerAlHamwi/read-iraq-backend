using Abp.Domain.Services;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributesForSourceType
{
    public interface IAttributeForSourceTypeManager : IDomainService
    {
        Task<AttributeForSourceType> GetEntityByIdAsync(int Id);
        Task SoftDeleteForEntityTranslation(List<AttributeForSourceTypeTranslation> translations);
        Task<bool> CheckAttributeForSourceTypeIsCorrect(List<CreateAttributeForSourceTypeValueDto> AttributeForSourceTypeValues);
        Task CheckIfAttribteChoiceHasPoints(int sourceTypeId);

    }
}

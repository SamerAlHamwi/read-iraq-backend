using Abp.Domain.Services;
using ReadIraq.Domain.SourceTypes.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.SourceTypes
{
    public interface ISourceTypeManager : IDomainService
    {
        Task<SourceType> GetLiteEntityByIdAsync(int id);
        Task<SourceTypeDto> GetEntityDtoByIdAsync(int id);
        Task HardDeleteForEntityTranslation(List<SourceTypeTranslation> translations);
        Task<bool> CheckIfEntityExict(int id);
        Task<List<SourceTypeDto>> GetAllSourceTypesIntoDto();
        Task DeleteAllAttributeInSourceTypeBySourceTypeId(int sourceTypeId);
        Task SoftDeleteForEntityTranslation(List<SourceTypeTranslation> translations);
        Task<SourceType> GetEntityByIdAsync(int id);
        Task<bool> CheckIfSourceTypeIsExist(int sourceTypeId);
        Task<int> GetPointsToGetRequestBySourceTypeIdAsync(int sourceTypeId);
        Task<int> GetPointsToGiftCompanyBySourceTypeIdAsync(int sourceTypeId);
        Task CheckIfSourceTypeWithMainPointsToGiftOrNotByAttributeId(int attributeId);


    }
}

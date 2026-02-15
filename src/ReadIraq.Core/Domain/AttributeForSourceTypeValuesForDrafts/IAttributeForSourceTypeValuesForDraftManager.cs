using Abp.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts
{
    public interface IAttributeForSourceTypeValuesForDraftManager : IDomainService
    {
        //Task<List<AttributeForSourceTypeValueDto>> GetAllAttributeForSourceTypeValuesByDraftId(int draftId);
        Task HardDeleteEntity(List<AttributeForSourceTypeValuesForDraft> attributeForSourceTypeValuesForDraft);
        Task DeleteSourceTypeValuesForDraftByDraftsIds(List<int> draftsIds);
        Task DeleteSourceTypeValuesForDraftByDraftId(int draftId);
        //Task<List<AttributeForSourceTypeValueDto>> GetAllAttributeForSourceTypeValuesByDraftId(int draftId);

    }
}

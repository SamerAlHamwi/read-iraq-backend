using Abp.Domain.Services;
using ReadIraq.Domain.Drafts.Dtos;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.Drafts
{
    public interface IDraftManager : IDomainService
    {
        Task<DraftDetailsDto> GetEntityDtoByIdAsync(int id);
        Task DeleteAllOldDrafts();
        Task<bool> CheckIfUserCanAddNewDraft(long userId);
        Task UpdateAttacmentForDraft(int draftId, List<AttributeChoiceAndAttachmentDto> AttributeChoiceAndAttachments);
        Task UpdateContcatForDraft(List<CreateRequestForQuotationContactDto> draftContacts, Draft draft);
        Task<Draft> GetEntityById(int id);
        Task HardDeleteDraftById(int draftId);





    }
}

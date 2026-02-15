using Abp.Domain.Services;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReadIraq.Domain.ServiceValuesForDrafts
{
    public interface IServiceValuesForDraftManager : IDomainService
    {
        Task<bool> InsertServiceValuesForDraft(List<ServiceValuesForDraftDto> serviceValues, int draftId);
        Task HardDeleteServiceValues(List<ServiceValuesForDraft> serviceValues);
        Task DeleteServiceValuesByDraftsIds(List<int> draftsIds);
        Task DeleteServiceValuesByDraftId(int draftId);
        Task<List<ServiceDetailsDto>> GetFullServicesByDraftIdAsync(int id);
    }
}

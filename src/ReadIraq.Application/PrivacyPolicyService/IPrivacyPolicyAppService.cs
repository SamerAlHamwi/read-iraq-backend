using ReadIraq.CrudAppServiceBase;
using ReadIraq.PrivacyPolicyService.Dto;

namespace ReadIraq.PrivacyPolicyService
{
    public interface IPrivacyPolicyAppService : IReadIraqAsyncCrudAppService<PrivacyPolicyDetailsDto, int, LitePrivacyPolicyDto, PagedPrivacyPolicyResultRequestDto,
         CreatePrivacyPolicyDto, UpdatePrivacyPolicyDto>
    {
    }
}

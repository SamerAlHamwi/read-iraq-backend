using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PrivacyPolicyService.Dto
{
    public class PagedPrivacyPolicyResultRequestDto : PagedResultRequestDto
    {
        public bool IsForMoney { get; set; } = false;
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int OrderNo { get; set; }
    }
}

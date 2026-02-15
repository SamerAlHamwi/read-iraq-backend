using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PrivacyPolicyService.Dto
{
    public class PrivacyPolicyDetailsDto : EntityDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<PrivacyPolicyTranslationDto> Translations { get; set; }
        public bool IsForMoney { get; set; }
        public bool IsActive { get; set; }
        public AppType App { get; set; }
        public int OrderNo { get; set; }
    }
}

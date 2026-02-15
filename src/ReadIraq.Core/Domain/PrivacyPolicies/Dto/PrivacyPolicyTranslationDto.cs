using Abp.AutoMapper;
using ReadIraq.Domain.PrivacyPolicies;

namespace ReadIraq.PrivacyPolicyService.Dto
{
    [AutoMap(typeof(PrivacyPolicyTranslation))]

    public class PrivacyPolicyTranslationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }

    }
}

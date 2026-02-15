using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.PrivacyPolicies
{
    public class PrivacyPolicyTranslation : FullAuditedEntity, IEntityTranslation<PrivacyPolicy>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public PrivacyPolicy Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

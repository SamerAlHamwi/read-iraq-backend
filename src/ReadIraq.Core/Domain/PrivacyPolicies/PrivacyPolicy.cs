using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.PrivacyPolicies
{
    public class PrivacyPolicy : FullAuditedEntity, IMultiLingualEntity<PrivacyPolicyTranslation>, IActiveEntity
    {
        public ICollection<PrivacyPolicyTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        public bool IsForMoney { get; set; }
        public AppType App { get; set; }
        public int OrderNo { get; set; }
    }
}

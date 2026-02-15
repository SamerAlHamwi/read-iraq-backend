using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RejectReasons
{
    public class RejectReason : FullAuditedEntity, IMultiLingualEntity<RejectReasonTranslation>
    {
        public ICollection<RejectReasonTranslation> Translations { get; set; }

        public PossibilityPotentialClient PossibilityPotentialClient { get; set; }
        public bool IsActive { get; set; }
    }
}

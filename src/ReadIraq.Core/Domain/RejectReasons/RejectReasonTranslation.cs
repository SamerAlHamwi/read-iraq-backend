using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;


namespace ReadIraq.Domain.RejectReasons
{
    public class RejectReasonTranslation : FullAuditedEntity, IEntityTranslation<RejectReason>
    {
        public string Description { get; set; }
        public RejectReason Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.SubServices
{
    public class SubServiceTranslation : FullAuditedEntity, IEntityTranslation<SubService>
    {
        public string Name { get; set; }
        public SubService Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

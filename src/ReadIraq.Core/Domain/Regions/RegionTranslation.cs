using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Regions
{
    public class RegionTranslation : FullAuditedEntity, IEntityTranslation<Region>
    {
        public string Name { get; set; }
        public Region Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.SourceTypes
{
    public class SourceTypeTranslation : FullAuditedEntity, IEntityTranslation<SourceType>
    {
        public string Name { get; set; }
        public SourceType Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

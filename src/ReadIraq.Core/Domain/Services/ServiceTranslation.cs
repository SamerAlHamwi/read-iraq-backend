using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.services
{
    public class ServiceTranslation : FullAuditedEntity, IEntityTranslation<Service>
    {
        public string Name { get; set; }
        public Service Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Advertisiments
{
    public class Advertisiment : FullAuditedEntity, IActiveEntity
    {
        public bool IsActive { get; set; }
        public string Link { get; set; }
        public bool ForSettings { get; set; }
    }
}

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Toolss
{
    public class ToolTranslation : FullAuditedEntity, IEntityTranslation<Tool>
    {
        public string Name { get; set; }
        public Tool Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.AttributeChoices
{
    public class AttributeChoiceTranslation : FullAuditedEntity, IEntityTranslation<AttributeChoice>
    {
        public string Name { get; set; }
        public AttributeChoice Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

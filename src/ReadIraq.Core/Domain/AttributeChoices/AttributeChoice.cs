using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.AttributesForSourceType;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.AttributeChoices
{
    public class AttributeChoice : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<AttributeChoiceTranslation>
    {
        public AttributeChoice()
        {
            Translations = new HashSet<AttributeChoiceTranslation>();
        }
        public bool IsActive { get; set; }
        public int? AttributeChociceParentId { get; set; }

        [ForeignKey(nameof(AttributeChociceParentId))]
        public virtual AttributeChoice ParentChoice { get; set; }
        public int? AttributeForSourceTypeId { get; set; }
        [ForeignKey(nameof(AttributeForSourceTypeId))]
        public virtual AttributeForSourceType Attribute { get; set; }
        public ICollection<AttributeChoiceTranslation> Translations { get; set; }
        public bool IsAttributeChoiceParent { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }

    }
}

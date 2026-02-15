using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.SourceTypes;
using System.Collections.Generic;

namespace ReadIraq.Domain.AttributesForSourceType
{
    public class AttributeForSourceType : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<AttributeForSourceTypeTranslation>
    {

        public ICollection<SourceType> SourceTypes { get; set; }
        public ICollection<AttributeChoice> AttributeChoices { get; set; }
        public ICollection<AttributeForSourceTypeTranslation> Translations { get; set; }
        public bool IsActive { get; set; }

    }
}

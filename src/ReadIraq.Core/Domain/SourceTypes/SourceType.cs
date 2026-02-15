using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.AttributesForSourceType;
using System.Collections.Generic;

namespace ReadIraq.Domain.SourceTypes
{
    public class SourceType : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<SourceTypeTranslation>
    {
        public ICollection<SourceTypeTranslation> Translations { get; set; }
        public ICollection<AttributeForSourceType> Attributes { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }
        public bool IsMainForPoints { get; set; }
        public bool IsActive { get; set; }
    }
}

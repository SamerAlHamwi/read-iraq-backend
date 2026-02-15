using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Terms
{
    public class Term : FullAuditedEntity, IMultiLingualEntity<TermTranslation>, IActiveEntity
    {
        public ICollection<TermTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        public AppType App { get; set; }

    }
}

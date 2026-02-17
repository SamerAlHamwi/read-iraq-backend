using Abp.Domain.Entities.Auditing;
using System;

namespace ReadIraq.Domain.Translations
{
    public class Translation : CreationAuditedEntity<Guid>
    {
        public string Code { get; set; } // en, ar, tr
        public string Name { get; set; } // Word
    }
}

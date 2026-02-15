using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.FrequentlyQuestions
{
    public class FrequentlyQuestion : FullAuditedEntity, IMultiLingualEntity<FrequentlyQuestionTranslation>, IActiveEntity
    {
        public ICollection<FrequentlyQuestionTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        public AppType App { get; set; }

    }
}

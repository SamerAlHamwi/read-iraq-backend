using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Drafts;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts
{
    public class AttributeForSourceTypeValuesForDraft : FullAuditedEntity
    {
        public int DraftId { get; set; }
        [ForeignKey(nameof(DraftId))]
        public Draft Draft { get; set; }
        public int AttributeForSourcTypeId { get; set; }
        public int AttributeChoiceId { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Drafts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.AttributeAndAttachmentsForDrafts
{
    public class AttributeAndAttachmentsForDraft : FullAuditedEntity
    {
        public int DraftId { get; set; }
        [ForeignKey(nameof(DraftId))]
        public virtual Draft Draft { get; set; }
        public int? AttributeChoiceId { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
    }
}

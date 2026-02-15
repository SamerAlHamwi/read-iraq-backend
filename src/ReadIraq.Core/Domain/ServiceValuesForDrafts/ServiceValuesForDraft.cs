using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Drafts;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.ServiceValuesForDrafts
{
    public class ServiceValuesForDraft : FullAuditedEntity
    {
        public int DraftId { get; set; }
        [ForeignKey(nameof(DraftId))]
        public virtual Draft Draft { get; set; }
        public int? ServiceId { get; set; }
        public int? SubServiceId { get; set; }
        public int? ToolId { get; set; }
    }
}

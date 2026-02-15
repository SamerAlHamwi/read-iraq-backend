using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Drafts;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotationContactsForDrafts
{
    public class RequestForQuotationContactsForDraft : FullAuditedEntity
    {
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string DailCode { get; set; }
        public bool IsWhatsAppAvailable { get; set; }
        public bool IsTelegramAvailable { get; set; }
        public bool IsCallAvailable { get; set; }
        public int DraftId { get; set; }
        [ForeignKey(nameof(DraftId))]
        public virtual Draft Draft { get; set; }
        public RequestForQuotationContactType RequestForQuotationContactType { get; set; }
    }
}

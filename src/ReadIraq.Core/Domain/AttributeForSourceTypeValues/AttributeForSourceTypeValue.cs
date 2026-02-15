using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.AttributesForSourceType;
using ReadIraq.Domain.RequestForQuotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.AttributeForSourcTypeValues
{
    public class AttributeForSourceTypeValue : FullAuditedEntity
    {
        public int AttributeForSourcTypeId { get; set; }
        [ForeignKey(nameof(AttributeForSourcTypeId))]
        public virtual AttributeForSourceType AttributeForSourcType { get; set; }
        public long RequestForQuotationId { get; set; }
        [ForeignKey(nameof(RequestForQuotationId))]
        public virtual RequestForQuotation RequestForQuotation { get; set; }
        public int? AttributeChoiceId { get; set; }
        [ForeignKey(nameof(AttributeChoiceId))]
        public virtual AttributeChoice AttributeChoice { get; set; }
    }
}

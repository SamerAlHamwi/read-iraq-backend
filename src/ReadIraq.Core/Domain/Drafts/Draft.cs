using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.AttributeAndAttachmentsForDrafts;
using ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts;
using ReadIraq.Domain.RequestForQuotationContactsForDrafts;
using ReadIraq.Domain.ServiceValuesForDrafts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Drafts
{
    public class Draft : FullAuditedEntity
    {
        public ICollection<AttributeForSourceTypeValuesForDraft> AttributeForSourceTypeValues { get; set; } = new List<AttributeForSourceTypeValuesForDraft>();
        public int? SourceTypeId { get; set; }
        public double? SourceLongitude { get; set; }
        public double? SourceLatitude { get; set; }
        public int? SourceCityId { get; set; }
        public string SourceAddress { get; set; }
        public ICollection<RequestForQuotationContactsForDraft> RequestForQuotationContacts { get; set; } = new List<RequestForQuotationContactsForDraft>();
        public ICollection<ServiceValuesForDraft> Services { get; set; } = new List<ServiceValuesForDraft>();
        public DateTime? MoveAtUtc { get; set; }
        public double? DestinationLongitude { get; set; }
        public double? DestinationLatitude { get; set; }
        public int? DestinationCityId { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime? ArrivalAtUtc { get; set; }
        public string Comment { get; set; }
        public ServiceType? ServiceType { get; set; }
        public ICollection<AttributeAndAttachmentsForDraft> AttributeChoiceAndAttachments { get; set; } = new List<AttributeAndAttachmentsForDraft>();
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.AttributeAndAttachments;
using ReadIraq.Domain.AttributeForSourcTypeValues;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.RequestForQuotationContacts;
using ReadIraq.Domain.ServiceValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations;
public class RequestForQuotation : FullAuditedEntity<long>
{
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
    public int SourceTypeId { get; set; }
    public ICollection<AttributeForSourceTypeValue> AttributeForSourceTypeValues { get; set; }
    public double SourceLongitude { get; set; }
    public double SourceLatitude { get; set; }
    public int? SourceCityId { get; set; }
    [ForeignKey(nameof(SourceCityId))]
    public virtual City SourceCity { get; set; }
    public string SourceAddress { get; set; }
    public ICollection<RequestForQuotationContact> RequestForQuotationContacts { get; set; }
    public ICollection<ServiceValue> Services { get; set; }
    public ICollection<AttributeChoiceAndAttachment> AttributeChoiceAndAttachments { get; set; }
    public DateTime MoveAtUtc { get; set; }
    public double DestinationLongitude { get; set; }
    public double DestinationLatitude { get; set; }
    public int? DestinationCityId { get; set; }
    [ForeignKey(nameof(DestinationCityId))]
    public virtual City DestinationCity { get; set; }
    public string DestinationAddress { get; set; }
    public DateTime? ArrivalAtUtc { get; set; }
    public string Comment { get; set; }
    public RequestForQuotationStatues Statues { get; set; }
    public ServiceType ServiceType { get; set; }
    public string ReasonRefuse { get; set; }
    public DateTime? FinishedDate { get; set; }
    public DateTime? ConfirmFinishDateByCompany { get; set; }
    public DateTime? ConfirmFinishDateByUser { get; set; }
    public string ReasonOfNotFinish { get; set; }
    public DateTime? HadOffersDate { get; set; }
    public DateTime? PossibledDate { get; set; }
    public string SourcePlaceNameByGoogle { get; set; }
    public string DestinationPlaceNameByGoogle { get; set; }
}


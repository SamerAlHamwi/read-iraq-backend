using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using ReadIraq.Domain.Offers.Dto;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.SourceTypes.Dto;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class RequestForQuotationDetailsDto : AuditedEntityDto<long>
    {
        public SourceTypeDto SourceType { get; set; }
        public List<AttributeForSourceTypeValueDto> AttributeForSourceTypeValues { get; set; }
        public double SourceLongitude { get; set; }
        public double SourceLatitude { get; set; }
        public LiteCityDto SourceCity { get; set; }
        public string SourceAddress { get; set; }
        public List<RequestForQuotationContactDto> RequestForQuotationContacts { get; set; }
        public List<ServiceDetailsDto> Services { get; set; }
        public DateTime MoveAtUtc { get; set; }
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public LiteCityDto DestinationCity { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime? ArrivalAtUtc { get; set; }
        public string Comment { get; set; }
        public List<AttributeChoiceAndAttachmentDetailsDto> AttributeChoiceAndAttachments { get; set; }
        public List<LiteAttachmentDto> Attachments { get; set; } = new List<LiteAttachmentDto>();
        public List<LiteAttachmentDto> FinishedRequestAttachmentByCompany { get; set; } = new List<LiteAttachmentDto>();
        public RequestForQuotationStatues Statues { get; set; }
        public ServiceType ServiceType { get; set; }
        public LiteUserDto User { get; set; }
        public string ReasonRefuse { get; set; }
        public OfferIdAndStatusDto SelectedOfferIdAndStatus { get; set; } = null;
        public Guid? ProviderOfferId { get; set; } = null;
        public TinySelectedCompanyDto SelectedCompany { get; set; }
        public bool IsThisRequestOfferWithThisCompany { get; set; } = false;
        public bool IsThisCompnayProvideOfferWithThisRequest { get; set; } = false;
        public string SourcePlaceNameByGoogle { get; set; }
        public string DestinationPlaceNameByGoogle { get; set; }
        public int DiscountPercentageIfUserCancelHisRequest { get; set; }
        public bool IsWillBeDiscount => DateTime.UtcNow.AddHours(48) >= MoveAtUtc && Statues is not (RequestForQuotationStatues.Checking or RequestForQuotationStatues.Approved or RequestForQuotationStatues.HasOffers);
        public int PointsToBuyRequest => SourceType != null ? SourceType.IsMainForPoints ? SourceType.PointsToBuyRequest : AttributeForSourceTypeValues.Where(x => x.AttributeChoice != null).Select(x => x.AttributeChoice).Sum(x => x.PointsToBuyRequest) : 0;
        public OfferStatues OfferStatues { get; set; }
    }
}

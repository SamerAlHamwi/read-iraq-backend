using Abp.Runtime.Validation;
using ReadIraq.Domain.AttributeForSourceTypeValues.Dto;
using ReadIraq.Domain.RequestForQuotationContacts.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class CreateRequestForQuotationDto : ICustomValidate
    {
        public List<CreateAttributeForSourceTypeValueDto> AttributeForSourceTypeValues { get; set; }
        public int SourceTypeId { get; set; }
        public double SourceLongitude { get; set; }
        public double SourceLatitude { get; set; }
        public int SourceCityId { get; set; }
        public string SourceAddress { get; set; }
        public List<CreateRequestForQuotationContactDto> RequestForQuotationContacts { get; set; }
        public List<ServiceValuesDto> Services { get; set; }
        public DateTime MoveAtUtc { get; set; }
        public double DestinationLongitude { get; set; }
        public double DestinationLatitude { get; set; }
        public int DestinationCityId { get; set; }
        public string DestinationAddress { get; set; }
        public DateTime? ArrivalAtUtc { get; set; }
        public string Comment { get; set; }
        public ServiceType ServiceType { get; set; }
        public List<AttributeChoiceAndAttachmentDto> AttributeChoiceAndAttachments { get; set; }
        public long? UserId { get; set; }
        public int? DraftId { get; set; }
        public string SourcePlaceNameByGoogle { get; set; }
        public string DestinationPlaceNameByGoogle { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {

            if (RequestForQuotationContacts.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Source).Count() < 1)
                context.Results.Add(new ValidationResult("You Need To Add Contact For Source "));

            if (RequestForQuotationContacts.Any(x => string.IsNullOrEmpty(x.PhoneNumber)))
                context.Results.Add(new ValidationResult("You Need To Add PhoneNumber For Source Contact "));

            //if (RequestForQuotationContacts.Where(x => x.RequestForQuotationContactType == RequestForQuotationContactType.Destination).Count() < 1)
            //    context.Results.Add(new ValidationResult("You Need To Add Contact For Destination "));

            if (SourceCityId == 0)
                context.Results.Add(new ValidationResult("You Need To Add CityId For Source "));

            if (DestinationCityId == 0)
                context.Results.Add(new ValidationResult("You Need To Add CityId For Destination "));

            if (SourceTypeId == 0)
                context.Results.Add(new ValidationResult("You Need To Add SourceTypeId"));

            if (AttributeChoiceAndAttachments.SelectMany(x => x.AttachmentIds).Any(x => x == 0))
                context.Results.Add(new ValidationResult("0 Is Incomplete Value In Attachment"));

            if (Services is null || Services.Count() == 0)
                context.Results.Add(new ValidationResult("You Need To Add One Service At Least"));


        }
    }
}

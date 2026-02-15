using Abp.Runtime.Validation;
using ReadIraq.Domain.ServiceValues.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ReadIraq.Domain.Offers.Dto
{
    public class CreateOfferDto : ICustomValidate
    {
        public List<ServiceValueForOfferDto> ServiceValueForOffers { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public long RequestId { get; set; }
        public string Note { get; set; }
        public bool IsExtendStorage { get; set; }
        public double? PriceForOnDayStorage { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if ((ServiceValueForOffers.Any(x => x.ToolId.HasValue && !x.Amount.HasValue)) || (ServiceValueForOffers.Any(x => !x.ToolId.HasValue && x.Amount.HasValue)))
                context.Results.Add(new ValidationResult("Amount Cannot be Null If Tool IS Not Null Or Opposite"));
            if (RequestId is 0)
                context.Results.Add(new ValidationResult("RequestId Cannot be 0"));
            if (Price is 0)
                context.Results.Add(new ValidationResult("Price Cannot be 0"));
            if (IsExtendStorage && !PriceForOnDayStorage.HasValue)
                context.Results.Add(new ValidationResult("Price For Storage Is Requierd "));
        }
    }
}

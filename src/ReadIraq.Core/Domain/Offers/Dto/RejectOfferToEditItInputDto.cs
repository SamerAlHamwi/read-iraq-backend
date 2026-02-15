using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Offers.Dto
{
    public class RejectOfferToEditItInputDto : ICustomValidate
    {
        [Required]
        public Guid OfferId { get; set; }
        [Required]
        public string ReasonRefuse { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (string.IsNullOrEmpty(ReasonRefuse))
                context.Results.Add(new ValidationResult("ReasonRefuse is Requierd"));
        }
    }
}

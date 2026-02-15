using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class FinishRequestByUserDto : ICustomValidate
    {
        [Required]
        public long RequestId { get; set; }
        public RequestForQuotationStatues Statues { get; set; }
        public string ReasonOfNotFinish { get; set; }
        public Guid? OfferId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (RequestId == 0)
                context.Results.Add(new ValidationResult("You Need To Add RequestId "));
            if (Statues == RequestForQuotationStatues.FinishByUser && OfferId is null)
                context.Results.Add(new ValidationResult("You Need To Insert OfferId When Statues Is FinishByUser"));
        }
    }
}

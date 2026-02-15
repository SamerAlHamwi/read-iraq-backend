using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.Offers.Dto
{
    public class RejectOffersInputDto /*: ICustomValidate*/
    {
        public List<Guid> OffersIds { get; set; }

        public int RejectReasonId { get; set; }

        public string RejectReasonDescription { get; set; }


        //public void AddValidationErrors(CustomValidationContext context)
        //{
        //    if (OffersIds.Count != 3)
        //        context.Results.Add(new ValidationResult("OffersIds must be 3"));

        //}
    }
}

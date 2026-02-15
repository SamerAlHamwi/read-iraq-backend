using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Offers.Dto
{
    public class OfferIdAndStatusDto
    {
        public Guid? SelectedOfferId { get; set; }
        public OfferStatues OfferStatues { get; set; }
    }
}

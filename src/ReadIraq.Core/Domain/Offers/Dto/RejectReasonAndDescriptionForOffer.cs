using ReadIraq.Domain.RejectReasons.Dto;

namespace ReadIraq.Domain.Offers.Dto
{
    public class RejectReasonAndDescriptionForOffer
    {
        public RejectReasonDetailsDto RejectReason { get; set; }
        public string RejectReasonDescription { get; set; }
    }
}

using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class RequestCompanyCompanyBranchIdsDto
    {
        public long RequestId { get; set; }
        public int? CompanyId { get; set; }
        public int? CompanyBranchId { get; set; }
        public OfferStatues OfferStatues { get; set; }
    }
}

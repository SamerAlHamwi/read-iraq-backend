namespace ReadIraq.Domain.CompanyBranches.Dto
{
    public class RequestsForCompanyBranchCountDto
    {
        public int CompanyBranchId { get; set; }
        public LiteCompanyBranchDto CompanyBranch { get; set; }
        public int RequestForQuotationCount { get; set; }
    }
}

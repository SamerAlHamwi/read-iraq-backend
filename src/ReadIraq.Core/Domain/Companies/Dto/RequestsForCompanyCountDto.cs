namespace ReadIraq.Domain.Companies.Dto
{
    public class RequestsForCompanyCountDto
    {
        public int CompanyId { get; set; }
        public LiteCompanyDto Company { get; set; }
        public int RequestForQuotationCount { get; set; }
    }
}

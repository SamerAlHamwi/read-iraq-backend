using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches.Dto;
using ReadIraq.Domain.RequestForQuotations.Dto;

namespace ReadIraq.Domain.SelectedCompaniesByUsers.Dto
{
    public class SelectedCompaniesBySystemForRequestDto
    {
        public RequestForQuotationDto RequestForQuotation { get; set; }
        public CompanyDto Company { get; set; }
        public CompanyBranchDto CompanyBranch { get; set; }
    }
}

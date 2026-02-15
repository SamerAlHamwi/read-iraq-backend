using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CompanyContactDetailsDto : EntityDto
    {
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebSite { get; set; }
        public bool IsForBranchCompany { get; set; }
        public int NumberOfTransfers { get; set; }
    }
}

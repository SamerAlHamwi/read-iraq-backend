using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Companies
{
    public class CompanyContact : FullAuditedEntity
    {
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebSite { get; set; }

    }
}

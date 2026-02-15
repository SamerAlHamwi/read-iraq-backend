using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Codes;
using System.Collections.Generic;

namespace ReadIraq.Domain.Partners
{
    public class Partner : FullAuditedEntity, IActiveEntity
    {
        public string PartnerPhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Code> Codes { get; set; }

        public virtual ICollection<City> CitiesPartner { get; set; }
    }
}

using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Companies;
using System.Collections.Generic;

namespace ReadIraq.Domain.CommissionGroups
{
    public class CommissionGroup : FullAuditedEntity
    {
        public double Name { get; set; }
        public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
        public bool IsDefault { get; set; }
    }
}

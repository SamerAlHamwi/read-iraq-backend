using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.CompanyBranches
{
    public class CompanyBranchTranslation : FullAuditedEntity, IEntityTranslation<CompanyBranch>
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public CompanyBranch Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

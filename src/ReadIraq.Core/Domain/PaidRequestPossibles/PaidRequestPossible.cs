using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.RequestForQuotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.PaidRequestPossibles
{
    public class PaidRequestPossible : FullAuditedEntity
    {
        public long RequestId { get; set; }
        [ForeignKey(nameof(RequestId))]
        public RequestForQuotation RequestForQuotation { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
        public int? CompanyBranchId { get; set; }
        [ForeignKey(nameof(CompanyBranchId))]
        public virtual CompanyBranch CompanyBranch { get; set; }
        public int NumberOfPaidPoints { get; set; }
    }
}

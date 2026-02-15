using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.RequestForQuotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.SelectedCompaniesByUsers
{
    public class SelectedCompaniesBySystemForRequest : FullAuditedEntity<Guid>
    {
        public long RequestForQuotationId { get; set; }
        [ForeignKey(nameof(RequestForQuotationId))]
        public RequestForQuotation RequestForQuotation { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
        public int? CompanyBranchId { get; set; }
        [ForeignKey(nameof(CompanyBranchId))]
        public virtual CompanyBranch CompanyBranch { get; set; }
    }
}

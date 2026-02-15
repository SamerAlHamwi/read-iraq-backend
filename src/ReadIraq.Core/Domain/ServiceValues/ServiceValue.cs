using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.services;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.ServiceValues
{
    public class ServiceValue : FullAuditedEntity<long>
    {
        public long? RequestForQuotationId { get; set; }
        [ForeignKey(nameof(RequestForQuotationId))]
        public RequestForQuotation RequestForQuotation { get; set; }
        public ServiceValueType ServiceValueType { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
        public int? CompanyBranchId { get; set; }
        [ForeignKey(nameof(CompanyBranchId))]
        public virtual CompanyBranch CompanyBranche { get; set; }
        public int? ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public virtual Service Service { get; set; }
        public int? SubServiceId { get; set; }
        public int? ToolId { get; set; }
    }
}

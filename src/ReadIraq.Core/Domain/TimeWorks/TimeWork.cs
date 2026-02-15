using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.TimeWorks
{
    public class TimeWork : FullAuditedEntity
    {
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
        public int? CompanyBranchId { get; set; }
        [ForeignKey(nameof(CompanyBranchId))]
        public CompanyBranch CompanyBranch { get; set; }
        public DayOfWeek Day { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
    }
}

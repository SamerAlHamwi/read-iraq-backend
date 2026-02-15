using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.MoneyTransfers
{
    public class MoneyTransfer : FullAuditedEntity
    {
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
        public int? CompanyBranchId { get; set; }
        [ForeignKey(nameof(CompanyBranchId))]
        public CompanyBranch CompanyBranch { get; set; }
        public double Amount { get; set; }
        public ReasonOfPaid ReasonOfPaid { get; set; }
        public PaidStatues PaidStatues { get; set; }
        public PaidProvider PaidProvider { get; set; }
        public PaidDestination PaidDestination { get; set; }
        /// <summary>
        /// this is not for relationship , it's just for get amount 
        /// </summary>
        public string OfferId { get; set; }
    }
}

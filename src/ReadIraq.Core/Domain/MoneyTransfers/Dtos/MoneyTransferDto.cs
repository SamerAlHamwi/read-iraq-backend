using Abp.Application.Services.Dto;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.CompanyBranches.Dto;
using ReadIraq.Domain.UserVerficationCodes;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.MoneyTransfers.Dtos
{
    public class MoneyTransferDto : EntityDto
    {
        public LiteUserDto User { get; set; }
        public CompanyDto Company { get; set; }
        public CompanyBranchDto CompanyBranch { get; set; }
        public double Amount { get; set; }
        public ReasonOfPaid ReasonOfPaid { get; set; }
        public PaidStatues PaidStatues { get; set; }
        public PaidProvider PaidProvider { get; set; }
        public PaidDestination PaidDestination { get; set; }
    }
}

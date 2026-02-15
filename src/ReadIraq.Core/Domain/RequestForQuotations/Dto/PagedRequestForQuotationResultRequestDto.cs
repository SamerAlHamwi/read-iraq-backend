using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class PagedRequestForQuotationResultRequestDto : PagedResultRequestDto, ICustomValidate
    {
        public int? CompanyId { get; set; }
        public int? CompanyBranchId { get; set; }
        public bool OnlyMyRequests { get; set; }
        public RequestForQuotationStatues? Statues { get; set; }
        public ServiceType? ServiceType { get; set; }
        public int? SourceTypeId { get; set; }
        public DateTime? CreationTime { get; set; }
        public string Keyword { get; set; }
        public bool GetPossibleRequest { get; set; } = false;
        public bool ForBroker { get; set; } = false;
        public int? BrokerId { get; set; }
        public bool GetPaidRequestForThisCompanyOrBranch { get; set; } = false;
        public bool AcceptPossibleRequest { get; set; }
        public long? UserId { get; set; }
        public bool GetRequestsThatHasStorage { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (CompanyId.HasValue && CompanyId.Value == 0)
                context.Results.Add(new ValidationResult("0 not Allowed"));
            if (CompanyBranchId.HasValue && CompanyBranchId.Value == 0)
                context.Results.Add(new ValidationResult("0 not Allowed"));
            if ((GetPaidRequestForThisCompanyOrBranch && CompanyId is null) && (GetPaidRequestForThisCompanyOrBranch && CompanyBranchId is null))
                context.Results.Add(new ValidationResult("When You GetPaidRequestForThisCompanyOrBranch Is True You Need To Enter CompanyId Or CompanyBranchId"));
        }
    }
}

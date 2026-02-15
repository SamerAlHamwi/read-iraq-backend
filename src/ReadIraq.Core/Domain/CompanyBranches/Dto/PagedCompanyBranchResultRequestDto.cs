using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.CompanyBranches.Dto
{
    public class PagedCompanyBranchResultRequestDto : PagedResultRequestDto, ICustomValidate
    {
        public string Keyword { get; set; }
        public long? CompanyId { get; set; }
        public bool IsForFilter { get; set; } = false;
        public long RequestId { get; set; }
        public bool GetCompnyBranchesWithRequest { get; set; } = false;
        public bool WhichBoughtInfoContact { get; set; } = false;
        public ServiceType? ServiceType { get; set; }
        [JsonIgnore]
        public List<int> CompanyBranchIds { get; set; }
        public CompanyStatues? Statues { get; set; }
        public CompanyBranchStatues? BranchStatues { get; set; }
        public bool? IsFeature { get; set; }
        public bool GetBranchesWithoutCompany { get; set; } = false;
        public bool? AcceptRequests { get; set; }
        public bool? AcceptPossibleRequests { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (IsForFilter && RequestId is 0)
                context.Results.Add(new ValidationResult("if IsForFilter true , Request Must hava value"));
            if (GetCompnyBranchesWithRequest && RequestId is 0)
                context.Results.Add(new ValidationResult("if GetCompnyBranchesWithRequest true , Request Must hava value"));
            if (WhichBoughtInfoContact && RequestId is 0)
                context.Results.Add(new ValidationResult("if WhichBoughtInfoContact true , Request Must hava value"));
        }

    }

}

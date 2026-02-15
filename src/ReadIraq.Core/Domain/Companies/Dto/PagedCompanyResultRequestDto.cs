using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class PagedCompanyResultRequestDto : PagedResultRequestDto, ICustomValidate
    {
        public string Keyword { get; set; }
        public bool IsForFilter { get; set; } = false;
        public long RequestId { get; set; }
        public bool GetCompaniesWithRequest { get; set; } = false;
        public bool WhichBoughtInfoContact { get; set; } = false;
        ///// <summary>
        ///// NoModificationRequest = 1,
        ///// ModificationRequest=2,
        ///// NoModificationRequestAccept=3, 
        ///// NoModificationReject = 4
        ///// </summary>     
        ///// <param name="input"></param>
        ///// <returns></returns>
        //public AskEditStatus? AskEditStatus { get; set; }
        public ServiceType? ServiceType { get; set; }
        public CompanyStatues? statues { get; set; }
        [JsonIgnore]
        public List<int> CompanyIds { get; set; }
        public bool? AcceptRequests { get; set; }
        public bool? AcceptPossibleRequests { get; set; }
        public bool? IsFeature { get; set; }
        public bool GetCompaniesThatNeedToUpdate { get; set; } = false;
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (IsForFilter && RequestId is 0)
                context.Results.Add(new ValidationResult("if IsForFilter true , Request Must hava value"));
            if (GetCompaniesWithRequest && RequestId is 0)
                context.Results.Add(new ValidationResult("if GetCompaniesWithRequest true , Request Must hava value"));
            if (WhichBoughtInfoContact && RequestId is 0)
                context.Results.Add(new ValidationResult("if WhichBoughtInfoContact true , Request Must hava value"));
        }
    }
}

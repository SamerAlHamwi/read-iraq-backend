using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CompanyStatuesDto : ICustomValidate
    {
        public int CompanyId { get; set; }
        public CompanyStatues Statues { get; set; }
        public string ReasonRefuse { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if ((Statues == CompanyStatues.Rejected || Statues == CompanyStatues.RejectedNeedToEdit) && string.IsNullOrEmpty(ReasonRefuse))
                context.Results.Add(new ValidationResult("ReasonRefuse is Requierd"));

        }
    }
    public class CompanyBranchStatuesDto : ICustomValidate
    {
        public int CompanyBranchId { get; set; }
        public CompanyBranchStatues Statues { get; set; }
        public string ReasonRefuse { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if ((Statues == CompanyBranchStatues.Rejected || Statues == CompanyBranchStatues.RejectedNeedToEdit) && string.IsNullOrEmpty(ReasonRefuse))
                context.Results.Add(new ValidationResult("ReasonRefuse is Requierd"));

        }
    }

}

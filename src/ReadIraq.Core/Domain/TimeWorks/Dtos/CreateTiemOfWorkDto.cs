using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ReadIraq.Domain.TimeWorks.Dtos
{
    public class CreateTiemOfWorkDto : ICustomValidate
    {
        public int? CompanyId { get; set; }
        public int? CompanyBranchId { get; set; }
        public List<TimeOfWorkDto> Timeworks { get; set; } = new List<TimeOfWorkDto>();

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (CompanyId.HasValue && CompanyBranchId.HasValue)
                context.Results.Add(new ValidationResult("You Must Enter Just One. CmpanyId Or CompanyBranch Id"));
            if (!CompanyId.HasValue && !CompanyBranchId.HasValue)
                context.Results.Add(new ValidationResult("You Must Enter Just One. CmpanyId Or CompanyBranch Id"));
            if (Timeworks.GroupBy(t => t.Day).Any(g => g.Count() > 1))
                context.Results.Add(new ValidationResult("You Cannot insert More than One Date at same Day"));
        }
    }
}

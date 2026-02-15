using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class RequestAndSelectedCompaniesIdsDto : ICustomValidate
    {
        [Required]
        public long RequestForQuotationId { get; set; }
        [Required]
        public List<int> CompaniesIds { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (RequestForQuotationId == 0)
                context.Results.Add(new ValidationResult("RequestForQuotationId Cannot Be 0"));
            if (CompaniesIds is null || CompaniesIds.Any(x => x == 0))
                context.Results.Add(new ValidationResult("CompaniesIds Cannot Be Null , Or Any Id = 0 !"));

        }
    }
}

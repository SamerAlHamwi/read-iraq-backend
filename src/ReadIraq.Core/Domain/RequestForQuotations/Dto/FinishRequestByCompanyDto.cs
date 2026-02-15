using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class FinishRequestByCompanyDto : ICustomValidate
    {
        public long RequestId { get; set; }
        public int CompanyOrCompanyBranchId { get; set; }
        [Required]
        public Provider Provider { get; set; }
        public List<long> AttachmentIdsForFinishedRequest { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (RequestId == 0)
                context.Results.Add(new ValidationResult("You Need To Add RequestId "));
            if (CompanyOrCompanyBranchId == 0)
                context.Results.Add(new ValidationResult("You Need To Add CompanyOrCompanyBranchId "));
            if (AttachmentIdsForFinishedRequest is null || AttachmentIdsForFinishedRequest.Count() < 3)
                context.Results.Add(new ValidationResult("You Need To Add At Lest 3 Attachment For Confirm Finish Requst By Company "));
        }
    }
}

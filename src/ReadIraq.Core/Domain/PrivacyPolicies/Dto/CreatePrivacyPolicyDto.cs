using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.PrivacyPolicyService.Dto
{
    public class CreatePrivacyPolicyDto : ICustomValidate
    {
        [Required]
        public List<PrivacyPolicyTranslationDto> Translations { get; set; }
        public bool IsForMoney { get; set; }
        public bool IsActive { get; set; }
        [Required]
        public AppType App { get; set; }
        public int OrderNo { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
        }
    }
}

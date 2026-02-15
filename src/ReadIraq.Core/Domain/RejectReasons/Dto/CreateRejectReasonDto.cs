using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RejectReasons.Dto
{
    public class CreateRejectReasonDto : ICustomValidate
    {

        public List<RejectReasonTranslationDto> Translations { get; set; }
        [Required]
        public PossibilityPotentialClient PossibilityPotentialClient { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));

        }
    }
}

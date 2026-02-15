using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.TermService.Dto
{
    public class CreateTermDto : ICustomValidate
    {
        [Required]
        public List<TermTranslationDto> Translations { get; set; }
        [Required]
        public AppType App { get; set; }


        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
        }
    }
}

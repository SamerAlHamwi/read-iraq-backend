using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.SubServices.Dto
{
    public class CreateSubServiceDto : ICustomValidate
    {

        [Required]
        public List<SubServiceTranslationDto> Translations { get; set; }
        [Required]
        public int ServiceId { get; set; }
        public long AttachmentId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
            if (ServiceId is 0)
                context.Results.Add(new ValidationResult("Service Id Must Not Be 0 "));
        }
    }
}

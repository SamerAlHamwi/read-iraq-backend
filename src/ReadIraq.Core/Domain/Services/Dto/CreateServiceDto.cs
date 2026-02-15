using Abp.Runtime.Validation;
using ReadIraq.Domain.Services.Dto;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.services.Dto
{
    public class CreateServiceDto : ICustomValidate
    {
        [Required]
        public long AttachmentId { get; set; }
        public List<ServiceTranslationDto> Translations { get; set; }
        public bool IsForStorage { get; set; }
        public bool IsForTruck { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
            if (AttachmentId is 0)
                context.Results.Add(new ValidationResult("Attachment Id Must not Be 0 "));
        }
    }
}

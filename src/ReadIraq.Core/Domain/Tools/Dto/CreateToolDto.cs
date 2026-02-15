using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class CreateToolDto : ICustomValidate
    {

        [Required]
        public List<ToolsTranslationDto> Translations { get; set; }
        [Required]
        public int SubServiceId { get; set; }
        [Required]
        public long AttachmentId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (SubServiceId is 0)
                context.Results.Add(new ValidationResult("Tool Cannot Be 0"));
            if (AttachmentId is 0)
                context.Results.Add(new ValidationResult("AttachmentId Mustnot be 0"));

        }
    }

    public class CreateToolWithAttributeIdDto : CreateToolDto
    {

        [Required]
        public int AttributeId { get; set; }

    }
}

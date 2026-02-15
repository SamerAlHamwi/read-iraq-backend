using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    public class UpdateAttributeChoiceDto : CreateAttributeChoiceDto, IEntityDto
    {
        [Required]
        public int Id { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Id is 0)
                context.Results.Add(new ValidationResult("Id Cannot Be 0 "));

        }
    }
}

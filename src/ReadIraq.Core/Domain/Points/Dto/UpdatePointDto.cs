using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Points.Dto
{
    public class UpdatePointDto : CreatePointDto, IEntityDto, ICustomValidate
    {
        [Required]
        public int Id { get; set; }

        public override void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
        }
    }
}

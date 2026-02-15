using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Countries.Dto
{
    public class UpdateCountryDto : CreateCountryDto, IEntityDto, ICustomValidate
    {
        [Required]
        public int Id { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Id == 0)
                context.Results.Add(new ValidationResult("Id must has value"));
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
        }
    }
}

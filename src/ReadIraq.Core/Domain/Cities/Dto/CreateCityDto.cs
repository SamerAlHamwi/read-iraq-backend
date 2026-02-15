using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Cities.Dto
{
    public class CreateCityDto
    {
        [Required]
        public List<CityTranslationDto> Translations { get; set; }
        [Required]
        public int CountryId { get; set; }
        public virtual void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
        }
    }
}

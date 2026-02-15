using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Points.Dto
{
    public class CreatePointDto
    {
        [Required]
        public List<PointTranslationDto> Translations { get; set; }
        public int NumberOfPoint { get; set; }
        public double Price { get; set; }
        public int NumberInMonths { get; set; }
        public bool IsForFeature { get; set; }
        public virtual void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
            if (IsForFeature && NumberOfPoint != 0)
                context.Results.Add(new ValidationResult("When You Create Feature Bundles You Cannot Make It As Poins"));
        }
    }
}

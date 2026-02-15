using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.SourceTypes.Dto
{
    public class CreateSourceTypeDto : ICustomValidate
    {

        [Required]
        public List<SourceTypeTranslationDto> Translations { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }
        [Required]
        public long IconId { get; set; }
        public List<int> AttributesIds { get; set; }
        public bool IsMainForPoints { get; set; }


        public virtual void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));

        }

    }
}

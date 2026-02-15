using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    public class CreateAttributeChoiceDto : ICustomValidate
    {

        [Required]
        public List<AttributeChoiceTranslationDto> Translations { get; set; }
        public int? AttributeId { get; set; }
        public bool IsAttributeChoiceParent { get; set; }
        public int? AttributeChociceParentId { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (AttributeId.HasValue && AttributeChociceParentId.HasValue && IsAttributeChoiceParent)
                context.Results.Add(new ValidationResult("AttributeChoice Must Be Parent Or Child "));
            if (!AttributeId.HasValue && !AttributeChociceParentId.HasValue)
                context.Results.Add(new ValidationResult("AttributeChoice Must Be Parent Or Child "));
            if (!AttributeId.HasValue && AttributeChociceParentId.HasValue && IsAttributeChoiceParent)
                context.Results.Add(new ValidationResult("AttributeChoice Must Be Parent Or Child "));

        }

    }

    public class CreateAttributeChoiceWithAttributeIdDto : CreateAttributeChoiceDto
    {

        [Required]
        public int AttributeId { get; set; }

    }
}

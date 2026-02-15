using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ReadIraq.Domain.AttributesForSourceType.Dto
{
    public class CreateAttributeForSourceTypeDto : ICustomValidate
    {
        public List<int> SourceTypeIds { get; set; }
        public List<AttributeForSourceTypeTranslationDto> Translations { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (SourceTypeIds.Count < 1 || SourceTypeIds.Any(x => x == 0) || SourceTypeIds is null)
                context.Results.Add(new ValidationResult("SourceTypeIds must contain at least One elements"));

        }
    }
}

using Abp.Application.Services.Dto;

using System.Collections.Generic;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    public class AttributeChoiceDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsAttributeChoiceParent { get; set; }
        public int AttributeChociceParentId { get; set; }

        public List<AttributeChoiceTranslationDto> Translations { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }
    }
}

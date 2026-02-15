using Abp.Application.Services.Dto;
using ReadIraq.Domain.AttributeChoices.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.AttributesForSourceType.Dto
{
    public class LiteAttributeForSourceTypeDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<AttributeForSourceTypeTranslationDto> Translations { get; set; }
        public List<AttributeChoiceDetailsDto> AttributeChoices { get; set; }
        public bool IsActive { get; set; }


    }
}

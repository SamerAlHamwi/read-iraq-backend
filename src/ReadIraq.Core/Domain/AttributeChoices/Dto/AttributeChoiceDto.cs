using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.AttributeChoices.Dto
{
    public class AttributeChoiceDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<AttributeChoiceTranslationDto> Translations { get; set; }
    }
}

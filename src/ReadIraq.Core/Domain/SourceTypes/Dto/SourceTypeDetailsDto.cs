using Abp.Application.Services.Dto;
using ReadIraq.Domain.AttributesForSourceType.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.SourceTypes.Dto
{
    public class SourceTypeDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public List<SourceTypeTranslationDto> Translations { get; set; }
        public LiteAttachmentDto Icon { get; set; } = new LiteAttachmentDto();
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }
        public List<AttributeForSourceTypeDto> Attributes { get; set; }
        public bool IsMainForPoints { get; set; }
        public bool IsActive { get; set; }


    }
}

using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.SourceTypes.Dto
{
    public class SourceTypeDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<SourceTypeTranslationDto> Translations { get; set; }
        public int PointsToGiftToCompany { get; set; }
        public int PointsToBuyRequest { get; set; }
        public int PointsToGiftMediator { get; set; }
        public bool IsMainForPoints { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        //public LiteAttachmentDto Photo { get; set; } = new LiteAttachmentDto();
    }
}

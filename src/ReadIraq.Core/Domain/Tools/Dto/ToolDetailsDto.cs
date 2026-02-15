using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class ToolDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<ToolsTranslationDto> Translations { get; set; }
        public LiteAttachmentDto Attachment { get; set; }

        public int? Amount { get; set; }

    }
}

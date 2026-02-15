using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class LiteToolDto : EntityDto<int>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<ToolsTranslationDto> Translations { get; set; }
        public LiteAttachmentDto Attachment { get; set; }

    }

    public class SuperLiteToolDto : EntityDto<int>
    {
        public int ToolId { get; set; }

    }



}

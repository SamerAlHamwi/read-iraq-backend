using Abp.Application.Services.Dto;
using ReadIraq.Domain.Toolss.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.SubServices.Dto
{
    public class SubServiceDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<SubServiceTranslationDto> Translations { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public List<ToolDetailsDto> Tools { get; set; }

    }
}

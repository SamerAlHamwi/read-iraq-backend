using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.SubServices.Dto
{
    public class LiteSubServiceDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<SubServiceTranslationDto> Translations { get; set; }
        public LiteAttachmentDto Attachment { get; set; }

    }
}

using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class ToolDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<ToolsTranslationDto> Translations { get; set; }
    }
}

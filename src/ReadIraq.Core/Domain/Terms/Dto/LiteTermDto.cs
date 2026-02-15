using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.TermService.Dto
{
    public class LiteTermDto : EntityDto<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<TermTranslationDto> Translations { get; set; }
        public bool IsActive { get; set; }
        public AppType App { get; set; }

    }
}

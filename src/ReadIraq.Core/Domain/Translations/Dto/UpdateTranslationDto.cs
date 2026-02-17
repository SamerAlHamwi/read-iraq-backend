using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Domain.Translations.Dto
{
    public class UpdateTranslationDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}

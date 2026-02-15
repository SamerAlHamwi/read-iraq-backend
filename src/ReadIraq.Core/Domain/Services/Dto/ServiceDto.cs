using Abp.Application.Services.Dto;
using ReadIraq.Domain.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.services.Dto
{
    public class ServiceDto : EntityDto<int>
    {
        public string Name { get; set; }
        public List<ServiceTranslationDto> Translations { get; set; }
    }
}

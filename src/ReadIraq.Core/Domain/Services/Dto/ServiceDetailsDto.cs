using Abp.Application.Services.Dto;
using ReadIraq.Domain.Services.Dto;
using ReadIraq.Domain.SubServices.Dto;
using System.Collections.Generic;

namespace ReadIraq.Domain.services.Dto
{
    public class ServiceDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public List<ServiceTranslationDto> Translations { get; set; } = new List<ServiceTranslationDto>();
        public LiteAttachmentDto Attachment { get; set; }
        public bool IsForStorage { get; set; }
        public bool IsForTruck { get; set; }
        public List<SubServiceDetailsDto> SubServices { get; set; } = new List<SubServiceDetailsDto>();
        public bool Active { get; set; }

    }
}

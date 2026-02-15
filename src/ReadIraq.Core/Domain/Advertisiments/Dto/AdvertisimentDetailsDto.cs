using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Advertisiments.Dto
{
    public class AdvertisimentDetailsDto : EntityDto
    {
        public List<AdvertisimentPositionDto> AdvertisimentPositions { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public string? Link { get; set; }
        public long? PropertyId { get; set; }
        public bool ForSettings { get; set; }
        public long? CreatorUserId { get; set; }
        public string CreatorUserName { get; set; }
        public bool? IsForCar { get; set; }
        public bool? IsForProperty { get; set; }
    }
}

using Abp.Application.Services.Dto;
using System.Collections.Generic;

namespace ReadIraq.Advertisiments.Dto
{
    public class LiteAdvertisimentDto : EntityDto<int>
    {
        public List<AdvertisimentPositionDto> AdvertisimentPositions { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public string? Link { get; set; }
        public bool ForSettings { get; set; }
        public long? CreatorUserId { get; set; }
        public string CreatorUserName { get; set; }

    }
}

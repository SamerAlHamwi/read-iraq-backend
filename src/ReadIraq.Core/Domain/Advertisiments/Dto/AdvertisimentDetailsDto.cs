using Abp.Application.Services.Dto;

namespace ReadIraq.Advertisiments.Dto
{
    public class AdvertisimentDetailsDto : EntityDto
    {
        public LiteAttachmentDto Attachment { get; set; }
        public string? Link { get; set; }
        public bool ForSettings { get; set; }
        public long? CreatorUserId { get; set; }
        public string CreatorUserName { get; set; }
    }
}

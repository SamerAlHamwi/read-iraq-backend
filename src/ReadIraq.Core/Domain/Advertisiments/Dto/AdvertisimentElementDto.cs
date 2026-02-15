namespace ReadIraq.Advertisiments.Dto
{
    public class AdvertisimentElementDto
    {
        public int Id { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public string? Link { get; set; }
        public bool ForSettings { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel;

namespace ReadIraq.Advertisiments.Dto
{
    public class CreateAdvertisimentDto
    {
        public long AttachmentId { get; set; }
        public string? Link { get; set; }
        [DefaultValue(false)]
        public bool ForSettings { get; set; }
        public List<CreateAdvertisimentPositionDto> AdvertisimentPositions { get; set; }
    }
}

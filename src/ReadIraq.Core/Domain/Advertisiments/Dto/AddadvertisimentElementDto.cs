using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Advertisiments.Dto
{
    public class AddadvertisimentElementDto : CreateAdvertisimentElementDto
    {
        [Required]
        public int AdvertisimentId { get; set; }
    }
}

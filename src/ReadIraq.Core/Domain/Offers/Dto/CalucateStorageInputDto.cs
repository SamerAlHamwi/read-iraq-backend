using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Offers.Dto
{
    public class CalucateStorageInputDto
    {
        [Required]
        public Guid OfferId { get; set; }
        [Required]
        public DateTime CurrentMoveArriveAt { get; set; }
        [Required]
        public DateTime RequestedMoveArriveAt { get; set; }


    }
}

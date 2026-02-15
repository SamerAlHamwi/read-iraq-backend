using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Offers.Dto
{
    public class OfferDto : EntityDto<int>
    {
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Note { get; set; }
        public string RejectReasonDescription { get; set; }

    }
}

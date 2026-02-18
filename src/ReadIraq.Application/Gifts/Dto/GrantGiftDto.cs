using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Gifts.Dto
{
    public class GrantGiftDto
    {
        [Required]
        public List<long> UserIds { get; set; }

        public Guid? PlanId { get; set; }

        public string Note { get; set; }
    }
}

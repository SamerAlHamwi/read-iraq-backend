using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Codes.Dto
{
    public class CreateCodeDto
    {
        [Required]
        [StringLength(8)]
        public string RSMCode { get; set; }

        public decimal DiscountPercentage { get; set; }

        public List<string> PhoneNumbers { get; set; }
        public CodeType CodeType { get; set; }
    }
}

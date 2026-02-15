using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class ChangeDateForRequestDto
    {
        [Required]
        public long RequestId { get; set; }
        [Required]
        public DateTime MoveArriveAt { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string PaymentMethodId { get; set; }
    }
}

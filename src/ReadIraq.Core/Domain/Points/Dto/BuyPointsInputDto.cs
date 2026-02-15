using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Points.Dto
{
    public class BuyPointsInputDto
    {
        [Required]
        public int Id { get; set; }
        public string PaymentMethodId { get; set; }

    }
}

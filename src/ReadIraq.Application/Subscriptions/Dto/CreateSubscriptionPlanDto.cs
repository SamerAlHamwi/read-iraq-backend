using Abp.AutoMapper;
using ReadIraq.Domain.Subscriptions;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Subscriptions.Dto
{
    [AutoMapTo(typeof(SubscriptionPlan))]
    public class CreateSubscriptionPlanDto
    {
        [Required]
        public string Name { get; set; }

        public int DurationDays { get; set; }

        public bool IsFeatured { get; set; }

        public decimal Price { get; set; }

        public decimal PriceBeforeDiscount { get; set; }

        public string DiscountText { get; set; }

        public string Currency { get; set; }
    }
}

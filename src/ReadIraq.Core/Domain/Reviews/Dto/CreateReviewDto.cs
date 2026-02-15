using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Reviews.Dto
{
    public class CreateReviewDto
    {

        [Required]
        public Guid OfferId { get; set; }

        [Range(1, 10, ErrorMessage = "Quality must be between 1 and 10."), Required]
        public double quality { get; set; }

        [Range(1, 10, ErrorMessage = "CustomerService must be between 1 and 10."), Required]
        public double CustomerService { get; set; }

        [Range(1, 10, ErrorMessage = "ValueOfServiceForMoney must be between 1 and 10."), Required]
        public double ValueOfServiceForMoney { get; set; }
        [Range(1, 10, ErrorMessage = "OverallRating must be between 1 and 10."), Required]
        public double OverallRating { get; set; }
        public string ReviewDescription { get; set; }
    }
}

using ReadIraq.Domain.UserVerficationCodes;

namespace ReadIraq.Domain.Reviews.Dto
{
    public class ReviewDetailsDto
    {
        public LiteUserDto User { get; set; }
        public string ReviewDescription { get; set; }
        public double Quality { get; set; }
        public double CustomerService { get; set; }
        public double ValueOfServiceForMoney { get; set; }

        public double OverallRating { get; set; }
    }
}

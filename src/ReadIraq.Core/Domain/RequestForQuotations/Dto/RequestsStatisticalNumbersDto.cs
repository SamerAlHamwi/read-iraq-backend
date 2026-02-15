namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class RequestsStatisticalNumbersDto
    {
        public int TotalNumber { get; set; }
        public int Checking { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Possible { get; set; }
        public int HasOffers { get; set; }
        public int InProcess { get; set; }

    }
}

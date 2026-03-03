namespace ReadIraq.ActivationCodes.Dto
{
    public class ActivationCodeStatisticsDto
    {
        public int TotalCodes { get; set; }
        public int UsedCodes { get; set; }
        public int UnusedCodes { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}

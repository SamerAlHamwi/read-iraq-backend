using ReadIraq.Domain.services.Dto;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class ServiceStatisticsForRequestsDto
    {
        public int ServiceId { get; set; }
        public LiteServiceDto Service { get; set; }
        public int RequestForQuotationCount { get; set; }
    }
}

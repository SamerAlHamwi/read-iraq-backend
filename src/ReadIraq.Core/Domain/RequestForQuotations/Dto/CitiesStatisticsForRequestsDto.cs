using ReadIraq.Cities.Dto;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class CitiesStatisticsForRequestsDto
    {
        public int CityId { get; set; }
        public LiteCityDto cityDto { get; set; }
        public int RequestForQuotationCount { get; set; }
    }
}

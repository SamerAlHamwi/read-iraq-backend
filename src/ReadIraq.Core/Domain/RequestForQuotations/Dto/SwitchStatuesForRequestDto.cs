using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class SwitchStatuesForRequestDto
    {
        public long RequestId { get; set; }
        public RequestForQuotationStatues Statues { get; set; }
        public string ReasonRefuse { get; set; }
    }
}

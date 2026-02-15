namespace ReadIraq.Domain.ServiceValues.Dto
{
    public class ServiceValuesDto
    {
        public int ServiceId { get; set; }
        public int SubServiceId { get; set; }
        public int? ToolId { get; set; }

    }
    public class ServiceValueForOfferDto : ServiceValuesDto
    {
        public int? Amount { get; set; }


    }
    public class ServiceValuesForDraftDto
    {
        public int? ServiceId { get; set; }
        public int? SubServiceId { get; set; }
        public int? ToolId { get; set; }

    }

}

using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class RequestForQuotationDto : EntityDto<int>
    {
        public LiteUserDto User { get; set; }
        public CityDto SourceCity { get; set; }
        public CityDto DestinationCity { get; set; }
        public DateTime MoveAtUtc { get; set; }
        public DateTime ArrivalAtUtc { get; set; }
        public RequestForQuotationStatues Statues { get; set; }
        public ServiceType ServiceType { get; set; }
        public string ReasonRefuse { get; set; }
    }
}

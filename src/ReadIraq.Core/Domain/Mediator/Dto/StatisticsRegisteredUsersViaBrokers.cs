using ReadIraq.Mediators.Dto;

namespace ReadIraq.Domain.Mediator.Dto
{
    public class StatisticsRegisteredUsersViaBrokers
    {
        public string Code { get; set; }
        public MediatorDetailsDto Broker { get; set; }
        public int UserCount { get; set; }
    }
}

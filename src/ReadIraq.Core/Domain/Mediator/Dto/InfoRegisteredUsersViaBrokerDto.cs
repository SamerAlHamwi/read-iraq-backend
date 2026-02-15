namespace ReadIraq.Domain.Mediator.Dto
{
    public class InfoRegisteredUsersViaBrokerDto
    {
        public int CountRegisteredUsers { get; set; }
        public int NumberServiceUsers { get; set; }
        public string BrokerCode { get; set; }
        public double MoneyOwed { get; set; }
    }
}

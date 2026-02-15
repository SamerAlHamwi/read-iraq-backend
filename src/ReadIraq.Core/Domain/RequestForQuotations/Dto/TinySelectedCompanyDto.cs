using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RequestForQuotations.Dto
{
    public class TinySelectedCompanyDto
    {
        public int Id { get; set; }
        public Provider Provider { get; set; }
    }
}

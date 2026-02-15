using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.PaidRequestPossibles.Dto
{
    public class CreateAskForContactDto
    {
        public int Id { get; set; }
        public long RequestId { get; set; }
        public Provider Provider { get; set; }
    }
}

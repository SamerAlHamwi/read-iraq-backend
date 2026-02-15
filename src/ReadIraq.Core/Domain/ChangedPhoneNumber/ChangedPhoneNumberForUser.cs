using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.ChangedPhoneNumber
{
    public class ChangedPhoneNumberForUser : FullAuditedEntity
    {
        public string NewPhoneNumber { get; set; }
        public string NewDialCode { get; set; }
        public long UserId { get; set; }
    }
}

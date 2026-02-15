using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.RegisterdPhoneNumbers
{
    public class RegisterdPhoneNumber : FullAuditedEntity<long>
    {
        public string DialCode { get; set; }
        public string PhoneNumber { get; set; }
        public string VerficationCode { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}

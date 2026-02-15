using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.ContactUses
{
    public class ContactUsTranslation : FullAuditedEntity, IEntityTranslation<ContactUs>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public ContactUs Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}
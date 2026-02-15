using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Terms
{
    public class TermTranslation : FullAuditedEntity, IEntityTranslation<Term>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Term Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

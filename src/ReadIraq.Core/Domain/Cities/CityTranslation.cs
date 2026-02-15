using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Cities
{
    public class CityTranslation : FullAuditedEntity, IEntityTranslation<City>
    {
        public string Name { get; set; }
        public City Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace ReadIraq.Domain.Points
{
    public class PointTranslation : FullAuditedEntity, IEntityTranslation<Point>
    {
        public string Name { get; set; }
        public Point Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }
    }
}

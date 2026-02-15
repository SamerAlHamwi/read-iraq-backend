using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace ReadIraq.Domain.Advertisiments
{
    public class Advertisiment : FullAuditedEntity, IActiveEntity
    {
        public bool IsActive { get; set; }
        public ICollection<AdvertisimentPosition> AdvertisimentPositions { get; set; }
        public string Link { get; set; }

        public bool ForSettings { get; set; }

    }
}

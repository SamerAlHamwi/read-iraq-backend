using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain.SubServices;
using System.Collections.Generic;

namespace ReadIraq.Domain.services
{
    public class Service : FullAuditedEntity, IMultiLingualEntity<ServiceTranslation>
    {
        public ICollection<SubService> SubServices { get; set; }
        public ICollection<ServiceTranslation> Translations { get; set; }
        public bool IsForStorage { get; set; }
        public bool IsForTruck { get; set; }
        public bool Active { get; set; }
        //public bool IsActive { get; set; }
    }
}

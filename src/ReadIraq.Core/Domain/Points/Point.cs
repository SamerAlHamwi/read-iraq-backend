using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.Collections.Generic;

namespace ReadIraq.Domain.Points
{
    public class Point : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<PointTranslation>
    {
        public int NumberOfPoint { get; set; }
        public double Price { get; set; }
        public virtual ICollection<PointTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// Here We Use Point Class To Buy a Feature Bundles 
        /// </summary>
        public int NumberInMonths { get; set; }
        public bool IsForFeature { get; set; }
    }
}

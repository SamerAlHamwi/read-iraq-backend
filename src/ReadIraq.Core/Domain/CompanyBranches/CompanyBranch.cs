using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.PointsValues;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.ServiceValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.CompanyBranches
{
    public class CompanyBranch : FullAuditedEntity, IMultiLingualEntity<CompanyBranchTranslation>
    {

        public int? RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
        public virtual Region Region { get; set; }
        public int? CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public virtual Company Company { get; set; }
        public int CompanyContactId { get; set; }
        [ForeignKey(nameof(CompanyContactId))]
        public CompanyContact CompanyContact { get; set; }
        public int NumberOfTransfers { get; set; }
        public virtual ICollection<ServiceValue> services { get; set; } = new List<ServiceValue>();
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<CompanyBranchTranslation> Translations { get; set; }
        public virtual ICollection<City> AvailableCities { get; set; }
        public ServiceType ServiceType { get; set; }
        public int NumberOfPaidPoints { get; set; }
        public int NumberOfGiftedPoints { get; set; }
        public virtual ICollection<PointsValue> PointsPurchased { get; set; }
        public bool AcceptRequests { get; set; }
        public bool AcceptPossibleRequests { get; set; }
        public bool IsFeature { get; set; }
        public DateTime? StartFeatureSubscribtionDate { get; set; }
        public DateTime? EndFeatureSubscribtionDate { get; set; }
        public CompanyBranchStatues? Statues { get; set; }
        public string ReasonRefuse { get; set; }

    }
}

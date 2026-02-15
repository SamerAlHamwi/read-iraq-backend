using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.PointsValues;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.ServiceValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies
{
    public class Company : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<CompanyTranslation>
    {
        public int NumberOfTransfers { get; set; }
        public virtual ICollection<ServiceValue> services { get; set; }
        public string Code { get; set; }
        public int? CompanyContactId { get; set; }
        [ForeignKey(nameof(CompanyContactId))]
        public virtual CompanyContact CompanyContact { get; set; }
        public virtual ICollection<CompanyBranch> CompanyBranches { get; set; }
        public virtual ICollection<City> AvailableCities { get; set; }
        public int? RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
        public virtual Region Region { get; set; }
        public long? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual ICollection<CompanyTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        public CompanyStatues statues { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Comment { get; set; }
        public int NumberOfPaidPoints { get; set; }
        public int NumberOfGiftedPoints { get; set; }
        public virtual ICollection<PointsValue> PointsPurchased { get; set; }
        public bool AcceptRequests { get; set; }
        public bool AcceptPossibleRequests { get; set; }
        public bool IsFeature { get; set; }
        public DateTime? StartFeatureSubscribtionDate { get; set; }
        public DateTime? EndFeatureSubscribtionDate { get; set; }
        public string ReasonRefuse { get; set; }
        public int? ParentCompanyId { get; set; }
    }
}

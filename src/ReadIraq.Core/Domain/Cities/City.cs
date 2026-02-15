using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Countries;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.Partners;
using ReadIraq.Domain.Regions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReadIraq.Domain.Cities
{
    //city model
    public class City : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<CityTranslation>
    {
        public City()
        {
            Translations = new HashSet<CityTranslation>();
        }
        public int CountryId { get; set; }
        [ForeignKey(nameof(CountryId))]
        public virtual Country Country { get; set; }
        public ICollection<CityTranslation> Translations { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<CompanyBranch> CompanyBranches { get; set; }
        public virtual ICollection<Partner> Partners { get; set; }
        public bool IsActive { get; set; }
    }
}

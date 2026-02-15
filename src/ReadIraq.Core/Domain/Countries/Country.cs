using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using ReadIraq.Domain;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Countries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Countries
{
    public class Country : FullAuditedEntity, IActiveEntity, IMultiLingualEntity<CountryTranslation>
    {
        public Country()
        {

            Cities = new HashSet<City>();
            Translations = new HashSet<CountryTranslation>();
        }

        [Required]
        [StringLength(5)]
        public string DialCode { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public ICollection<CountryTranslation> Translations { get; set; }
        public bool IsActive { get; set; }
        public ServiceType Type { get; set; }
    }
}

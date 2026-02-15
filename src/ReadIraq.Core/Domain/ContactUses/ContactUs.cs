using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.ContactUses
{
    public class ContactUs : FullAuditedEntity, IMultiLingualEntity<ContactUsTranslation>
    {
        public string PhoneNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string WhatsNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Instgram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DayOfWeek StartDay { get; set; }
        public DayOfWeek EndDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ICollection<ContactUsTranslation> Translations { get; set; }

    }
}

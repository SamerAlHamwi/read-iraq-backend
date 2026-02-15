using Abp.Application.Services.Dto;

using System;
using System.Collections.Generic;


namespace ReadIraq.ContactUsService.Dto
{
    public class ContactUsDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string WhatsNumber { get; set; }
        public string Instgram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public LiteAttachmentDto Attachment { get; set; } = new LiteAttachmentDto();
        public DateTime CreationTime { get; set; }
        public List<ContactUsTranslationDto> Translations { get; set; }
        /// <summary>
        ///Sunday = 0,
        ///Monday = 1,
        ///Tuesday = 2,
        ///Wednesday = 3,
        ///Thursday = 4,
        ///Friday = 5,
        ///Saturday = 6
        /// </summary>
        public DayOfWeek StartDay { get; set; }
        public DayOfWeek EndDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}

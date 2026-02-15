using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.ContactUsService.Dto
{
    public class CreateContactUsDto
    {
        /// <summary>
        /// Translations
        /// </summary>
        [Required]
        public List<ContactUsTranslationDto> Translations { get; set; }
        public int? AttachmentId { get; set; }
        public string EmailAddress { get; set; }
        public string Instgram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string PhoneNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string WhatsNumber { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DayOfWeek StartDay { get; set; }
        public DayOfWeek EndDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
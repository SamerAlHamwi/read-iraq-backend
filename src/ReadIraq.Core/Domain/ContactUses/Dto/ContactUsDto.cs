using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.ContactUses;
using System;


namespace ReadIraq.ContactUsService.Dto
{
    [AutoMap(typeof(ContactUs))]
    public class ContactUsDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string EmailAddress { get; set; }
        public string Instgram { get; set; }
        public string Facebook { get; set; }
        public string PhoneNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string WhatsNumber { get; set; }
        public string Twitter { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}

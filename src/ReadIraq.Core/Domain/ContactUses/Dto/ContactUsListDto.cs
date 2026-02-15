using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;

namespace ReadIraq.ContactUsService.Dto
{
    public class ContactUsListDto : EntityDto, IHasCreationTime
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public string Instgram { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime CreationTime { get; set; }

    }
}

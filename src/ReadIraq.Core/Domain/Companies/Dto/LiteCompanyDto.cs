using Abp.Application.Services.Dto;
using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class LiteCompanyDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<CompanyTranslationDto> Translations { get; set; }
        public int NumberOfTransfers { get; set; }
        public string Address { get; set; }
        public LiteRegionDto Region { get; set; }
        public LiteUserDto User { get; set; }
        public int Rate { get; set; }
        public bool IsActive { get; set; }
        public CompanyStatues statues { get; set; }
        public ServiceType ServiceType { get; set; }
        public LiteAttachmentDto CompanyProfile { get; set; }
        public List<ServiceDto> Services { get; set; }
        public int NumberOfPaidPoints { get; set; }
        public int NumberOfGiftedPoints { get; set; }
        public int TotalPoints { get; set; }
        public bool IsThisCompanyProvideOffer { get; set; } = false;
        public bool AcceptRequests { get; set; }
        public bool AcceptPossibleRequests { get; set; }
        public bool IsFeature { get; set; }
        public DateTime? StartFeatureSubscribtionDate { get; set; }
        public DateTime? EndFeatureSubscribtionDate { get; set; }
        public double CompatibilityRate { get; set; }
        public string CommissionGroup { get; set; }
    }

}

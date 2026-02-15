using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using ReadIraq.Domain.CompanyBranches.Dto;
using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.TimeWorks.Dtos;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CompanyDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public List<CompanyTranslationDto> Translations { get; set; }
        public int NumberOfTransfers { get; set; }
        public string Address { get; set; }
        public LiteRegionDto Region { get; set; }
        public int Rate { get; set; }
        public bool IsActive { get; set; }
        public CompanyContactDetailsDto CompanyContact { get; set; }
        public List<CompanyBranchDetailsDto> CompanyBranches { get; set; }
        public List<ServiceDetailsDto> Services { get; set; }
        public LiteAttachmentDto CompanyProfile { get; set; }
        public List<LiteAttachmentDto> CompanyOwnerIdentity { get; set; } = new List<LiteAttachmentDto>();
        public List<LiteAttachmentDto> CompanyCommercialRegister { get; set; } = new List<LiteAttachmentDto>();
        public List<LiteAttachmentDto> AdditionalAttachment { get; set; } = new List<LiteAttachmentDto>();
        public List<LiteCityDto> AvailableCities { get; set; }
        public LiteUserDto User { get; set; }
        public CompanyStatues statues { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Comment { get; set; }
        public GeneralRatingDto GeneralRating { get; set; }
        public int NumberOfPaidPoints { get; set; }
        public int NumberOfGiftedPoints { get; set; }
        public int TotalPoints { get; set; }
        public bool AcceptRequests { get; set; }
        public bool AcceptPossibleRequests { get; set; }
        public bool IsFeature { get; set; }
        public DateTime? StartFeatureSubscribtionDate { get; set; }
        public DateTime? EndFeatureSubscribtionDate { get; set; }
        public List<TimeOfWorkDto> TimeOfWorks { get; set; } = new List<TimeOfWorkDto>();
        public string ReasonRefuse { get; set; }
        public int? ParentCompanyId { get; set; }
        public CompanyStatuesDto ChildCompanyStatuesDto { get; set; }

    }
}

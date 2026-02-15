using Abp.Application.Services.Dto;
using ReadIraq.Cities.Dto;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Domain.services.Dto;
using ReadIraq.Domain.TimeWorks.Dtos;
using ReadIraq.Domain.UserVerficationCodes;
using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.CompanyBranches.Dto
{
    public class CompanyBranchDetailsDto : EntityDto
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Address { get; set; }
        public List<CompanyBranchTranslationDto> Translations { get; set; }
        public LiteRegionDto Region { get; set; }
        public CompanyContactDetailsDto CompanyContact { get; set; }
        public CompanyDto Company { get; set; }
        public LiteUserDto User { get; set; }
        public bool IsActive { get; set; }
        public List<LiteCityDto> AvailableCities { get; set; }
        public List<ServiceDetailsDto> Services { get; set; }
        public int NumberOfTransfers { get; set; }
        public ServiceType ServiceType { get; set; }
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
        public CompanyBranchStatues Statues { get; set; }
        public string ReasonRefuse { get; set; }

    }
}

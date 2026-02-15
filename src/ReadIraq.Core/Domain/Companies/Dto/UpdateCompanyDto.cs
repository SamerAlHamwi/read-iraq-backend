using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.TimeWorks.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class UpdateCompanyDto : IEntityDto, ICustomValidate
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public List<CompanyTranslationDto> Translations { get; set; }
        public List<ServiceValuesDto> services { get; set; }

        [Required]
        public int RegionId { get; set; }
        public CompanyContactDto CompanyContact { get; set; }
        //public List<CompanyBranchDto> CompanyBranches { get; set; }
        [Required]
        public long CompanyProfilePhotoId { get; set; }
        [Required]
        public List<long> CompanyOwnerIdentityIds { get; set; }
        public List<int> AvailableCitiesIds { get; set; } = new List<int>();
        [Required]
        public List<long> CompanyCommercialRegisterIds { get; set; }
        public List<long> AdditionalAttachmentIds { get; set; } = new List<long>();
        public ServiceType ServiceType { get; set; }
        public string Comment { get; set; }
        public List<TimeOfWorkDto> Timeworks { get; set; } = new List<TimeOfWorkDto>();
        public bool IsForRequestUpdate { get; set; }
        [JsonIgnore]
        public bool DeleteAllOldAttachments { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Translations is null || Translations.Count < 2)
                context.Results.Add(new ValidationResult("Translations must contain at least two elements"));
            if (Id == 0)
                context.Results.Add(new ValidationResult("Id Cannot be 0"));
            if (RegionId == 0)
                context.Results.Add(new ValidationResult("Region Id Cannot be 0"));
            if (CompanyProfilePhotoId == 0)
                context.Results.Add(new ValidationResult("CompanyProfilePhotoId Cannot be 0"));
            if (CompanyOwnerIdentityIds.Any(x => x == 0))
                context.Results.Add(new ValidationResult("CompanyOwnerIdentityId Cannot be 0"));
            if (AdditionalAttachmentIds is not null && AdditionalAttachmentIds.Count > 0 && AdditionalAttachmentIds.Any(x => x == 0))
                context.Results.Add(new ValidationResult("AdditionalAttachmentId Cannot be 0"));
            if (CompanyCommercialRegisterIds.Any(x => x == 0))
                context.Results.Add(new ValidationResult("CompanyCommercialRegisterId Cannot be 0"));
        }
    }
}

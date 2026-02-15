using Abp.Runtime.Validation;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.TimeWorks.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Companies.Dto
{
    public class CreateCompanyDto : ICustomValidate
    {
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
        public SuperLiteUserDto userDto { get; set; }
        public ServiceType ServiceType { get; set; }
        public string Comment { get; set; }
        public List<TimeOfWorkDto> Timeworks { get; set; } = new List<TimeOfWorkDto>();
        public int? OldCompanyId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
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
            if (Translations.Count < 2)
                context.Results.Add(new ValidationResult(" Translations must be at least 2 items"));
            //if (CompanyBranches is not null && CompanyBranches.Any(x => x.RegionId == 0))
            //    context.Results.Add(new ValidationResult(" Region Id In CompanyBranche Cannot be 0 "));
            if (Timeworks.GroupBy(t => t.Day).Any(g => g.Count() > 1))
                context.Results.Add(new ValidationResult("You Cannot insert More than One Date at same Day"));


        }
    }
}

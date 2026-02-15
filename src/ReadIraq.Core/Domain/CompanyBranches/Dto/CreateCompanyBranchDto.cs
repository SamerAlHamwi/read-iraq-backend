using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using ReadIraq.Domain.Companies.Dto;
using ReadIraq.Domain.ServiceValues.Dto;
using ReadIraq.Domain.TimeWorks.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.CompanyBranches.Dto
{
    public class CreateCompanyBranchDto : ICustomValidate
    {

        public int CompanyId { get; set; }
        public int RegionId { get; set; }
        public CompanyContactDto CompanyContact { get; set; }
        public List<ServiceValuesDto> services { get; set; }
        public List<CompanyBranchTranslationDto> Translations { get; set; }
        public SuperLiteUserDto userDto { get; set; } = new SuperLiteUserDto();
        public List<int> AvailableCitiesIds { get; set; } = new List<int>();
        public ServiceType ServiceType { get; set; }
        public List<TimeOfWorkDto> Timeworks { get; set; } = new List<TimeOfWorkDto>();
        public bool IsWithCompany { get; set; }
        public long? UserCompanyBranchId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (RegionId == 0)
                context.Results.Add(new ValidationResult("Region Id Cannot be 0"));
            if (CompanyId == 0 && IsWithCompany)
                context.Results.Add(new ValidationResult("CompanyId Cannot be 0"));
            if (!UserCompanyBranchId.HasValue && !IsWithCompany)
                context.Results.Add(new ValidationResult("UserId Cannot be 0"));
            if (string.IsNullOrEmpty(userDto.PhoneNumber) && IsWithCompany)
                context.Results.Add(new ValidationResult("PhoneNumber Is Required"));
            if (string.IsNullOrEmpty(userDto.EmailAddress) && IsWithCompany)
                context.Results.Add(new ValidationResult("EmailAddress Is Required"));
            if (string.IsNullOrEmpty(userDto.DialCode) && IsWithCompany)
                context.Results.Add(new ValidationResult("DialCode Is Required"));
            if (Timeworks.GroupBy(t => t.Day).Any(g => g.Count() > 1))
                context.Results.Add(new ValidationResult("You Cannot insert More than One Date at same Day"));
        }
    }
    public class UpdateCompanyBranchDto : CreateCompanyBranchDto, IEntityDto, ICustomValidate
    {
        public int Id { get; set; }
        private new SuperLiteUserDto userDto { get; set; }
        private new long? UserCompanyBranchId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Id == 0)
                context.Results.Add(new ValidationResult("Id Cannot be 0"));
            if (CompanyId == 0 && IsWithCompany)
                context.Results.Add(new ValidationResult("CompanyId Cannot be 0"));
        }

    }
}

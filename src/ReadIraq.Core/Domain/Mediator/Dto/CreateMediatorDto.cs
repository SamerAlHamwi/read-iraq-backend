using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Domain.Mediators.Dto
{
    public class CreateMediatorDto : ICustomValidate
    {
        public string MediatorCode { get; set; }
        public string MediatorPhoneNumber { get; set; }
        public string DialCode { get; set; }
        public decimal CommissionPercentage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string Email { get; set; }
        public int CityId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (CityId is 0)
                context.Results.Add(new ValidationResult("City Id Is Requierd"));
            if (string.IsNullOrEmpty(MediatorPhoneNumber))
                context.Results.Add(new ValidationResult("PhoneNumber Is Requierd"));
            if (string.IsNullOrEmpty(MediatorCode))
                context.Results.Add(new ValidationResult("MediatorCode Is Requierd"));
            if (string.IsNullOrEmpty(DialCode))
                context.Results.Add(new ValidationResult("DialCode Is Requierd"));
        }
    }
}

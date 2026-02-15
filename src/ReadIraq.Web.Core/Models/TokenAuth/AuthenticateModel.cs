using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Models.TokenAuth
{
    public class AuthenticateModel : ICustomValidate
    {
        [Required]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberClient { get; set; }
        public bool IsForCompany { get; set; }
        public bool IsForCompanyBranch { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (IsForCompany && IsForCompanyBranch)
                context.Results.Add(new ValidationResult("You Must Select Company Or Branch Only"));
        }
    }
}

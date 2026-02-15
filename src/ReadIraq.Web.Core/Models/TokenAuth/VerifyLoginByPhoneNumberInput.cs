using Abp.Runtime.Validation;
using ReadIraq.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Models.TokenAuth
{
    public class VerifyLoginByPhoneNumberInput : ICustomValidate
    {
        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string DialCode { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Code { get; set; }

        public string Password { get; set; }
        public bool IsForUpdatePassword { get; set; } = false;
        public bool IsFromBasicApp { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!IsForUpdatePassword && string.IsNullOrEmpty(Password))
                context.Results.Add(new ValidationResult("You Must Enter The Password"));
        }
    }
    public class VerifyChangePhoneNumberInput
    {
        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string DialCode { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Code { get; set; }
    }
    public class VerifyResetPasswordForUserInputDto
    {
        [Required]
        public string DialCode { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string PhoneNumber { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string NewPassword { get; set; }
        public bool IsFromBasicApp { get; set; }


    }
    public class VerifyResetPasswordForUserUsingEmailInputDto
    {
        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string EmailAddress { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string NewPassword { get; set; }
        public bool IsFromBasicApp { get; set; }


    }

    public class VerifySignUpByPhoneNumberInput : ICustomValidate
    {
        [Required]
        public string FullName { get; set; }
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string Email { get; set; }
        [Required]
        public string DialCode { get; set; }

        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string PhoneNumber { get; set; }
        [Required]
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string Password { get; set; }
        public bool IsForCompany { get; set; }
        public string MediatorCode { get; set; }
        public bool IsForCompanyBranch { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (IsForCompanyBranch && IsForCompany)
                context.Results.Add(new ValidationResult("You Must Select Company Or Branch."));

        }
    }

}

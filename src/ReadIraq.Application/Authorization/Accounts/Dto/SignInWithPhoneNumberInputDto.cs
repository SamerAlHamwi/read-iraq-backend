using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class SignInWithPhoneNumberInputDto
    {
        [Required]
        public string DialCode { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public bool IsFromBasicApp { get; set; }


    }

    public class VerifySignUpWithPhoneNumberInputDto : SignInWithPhoneNumberInputDto
    {
        [Required]
        public string Code { get; set; }
    }



    public class VerifiyPhoneNumberInputDto : SignInWithPhoneNumberInputDto
    {
        [Required]
        public bool IsForRegistiration { get; set; }
    }



}

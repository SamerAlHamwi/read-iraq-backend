using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class VerifySignUpInput
    {
        [Required]
        public string DialCode { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}

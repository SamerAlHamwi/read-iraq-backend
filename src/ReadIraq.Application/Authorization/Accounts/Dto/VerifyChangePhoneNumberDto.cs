using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class VerifyChangePhoneNumberDto
    {
        [Required]
        public string NewDialCode { get; set; }

        [Required]
        public string NewPhone { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}

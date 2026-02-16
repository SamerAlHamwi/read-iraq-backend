using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class ResendCodeInput
    {
        [Required]
        public string DialCode { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}

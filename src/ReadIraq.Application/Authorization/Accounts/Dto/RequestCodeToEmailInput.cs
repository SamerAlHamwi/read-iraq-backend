using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class RequestCodeToEmailInput
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public ConfirmationCodeType CodeType { get; set; }
        public bool IsFromBasicApp { get; set; }
    }

    public record ConfirmEmailWithCodeInputDto
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Code { get; set; }
        public bool IsFromBasicApp { get; set; }

    }
}

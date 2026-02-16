using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Accounts.Dto
{
    public class RegisterUserInput
    {
        [Required]
        public string DialCode { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string FullName { get; set; }

        public int? GradeId { get; set; }

        public int? GovernorateId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
}

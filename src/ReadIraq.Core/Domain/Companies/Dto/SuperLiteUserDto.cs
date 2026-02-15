using ReadIraq.Authorization.Users;

namespace ReadIraq.Domain.Companies.Dto
{
    public class SuperLiteUserDto
    {
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string DialCode { get; set; }
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string PhoneNumber { get; set; }
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string EmailAddress { get; set; }
        [NoWhiteSpace(ErrorMessage = "{0} cannot contain whitespace.")]
        public string Password { get; set; }
    }
}

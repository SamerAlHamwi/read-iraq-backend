namespace ReadIraq.Authorization.Accounts.Dto
{
    public class RegisterUserOutput
    {
        public string UserId { get; set; }
        public string Phone { get; set; }
        public bool OtpSent { get; set; }
        public string Message { get; set; }
    }
}

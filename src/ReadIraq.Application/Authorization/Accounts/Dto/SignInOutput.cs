namespace ReadIraq.Authorization.Accounts.Dto
{
    public class SignInOutput
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserDetailDto User { get; set; }
    }
}

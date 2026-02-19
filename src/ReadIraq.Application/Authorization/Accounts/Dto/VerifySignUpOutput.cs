namespace ReadIraq.Authorization.Accounts.Dto
{
    public class VerifySignUpOutput
    {
        public UserDetailDto User { get; set; }
        public string Token { get; set; }
    }
}

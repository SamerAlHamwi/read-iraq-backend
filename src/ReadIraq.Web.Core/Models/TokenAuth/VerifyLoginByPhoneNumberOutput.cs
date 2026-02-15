using static ReadIraq.Enums.Enum;

namespace ReadIraq.Models.TokenAuth
{
    public class VerifyLoginByPhoneNumberOutput
    {
        public string AccessToken { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
        public int ExpireInSeconds { get; set; }



    }
}

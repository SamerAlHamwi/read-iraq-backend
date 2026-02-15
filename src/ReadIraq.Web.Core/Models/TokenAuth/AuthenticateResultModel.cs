using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Models.TokenAuth
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }

        public string EncryptedAccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }
        public UserType UserType { get; set; }
        public int CompanyId { get; set; }
        public int CompanyBranchId { get; set; }
        public string Language { get; set; }
        public List<string> Permissions { get; set; }

    }
}

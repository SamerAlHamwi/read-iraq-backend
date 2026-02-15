using Abp.Authorization.Users;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Users
{

    public class User : AbpUser<User>
    {
        public const string DefaultPassword = "123qwe";
        public string DialCode { get; set; }
        public string RegistrationFullName { get; set; }
        public UserType Type { get; set; }
        public string FcmToken { get; set; }
        public string MediatorCode { get; set; }
        [Required]
        [StringLength(6)]
        public string PIN { get; set; }
        public new virtual bool IsLockoutEnabled { get; set; } = false;

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            return user;
        }



    }
}

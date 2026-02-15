using Abp.Authorization;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;

namespace ReadIraq.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}

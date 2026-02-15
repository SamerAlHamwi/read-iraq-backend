using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace ReadIraq.Controllers
{
    public abstract class ReadIraqControllerBase : AbpController
    {
        protected ReadIraqControllerBase()
        {
            LocalizationSourceName = ReadIraqConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ReadIraq.Configuration;

namespace ReadIraq.Web.Host.Startup
{
    [DependsOn(
       typeof(ReadIraqWebCoreModule))]
    public class ReadIraqWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ReadIraqWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ReadIraqWebHostModule).GetAssembly());
        }
    }
}

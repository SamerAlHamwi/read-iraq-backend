using ReadIraq.Configuration.Dto;
using System.Threading.Tasks;

namespace ReadIraq.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
        Task SetEmailSetting(EmailSettingDto input);

        Task<EmailSettingDto> GetEmailSetting();
        Task SetSmsSetting(SmsSettingDto input);
        Task<SmsSettingDto> GetSmsSetting();

        Task SetFileSizeSetting(FileSizeSettingDto input);
        Task<FileSizeSettingDto> GetFileSizeSetting();

    }
}

using Abp.Configuration;
using System.Collections.Generic;

namespace ReadIraq.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.FileSize, "5", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.ImageSize, "100", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.ApkSize, "100", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, clientVisibilityProvider: new VisibleSettingClientVisibilityProvider()),
                new SettingDefinition(AppSettingNames.NumberOfPointsToGitft, "5", scopes: SettingScopes.Application | SettingScopes.All | SettingScopes.User),
                new SettingDefinition(AppSettingNames.NumberOfPointsToGetFromCompanyToGetRequestContact, "20", scopes: SettingScopes.Application | SettingScopes.All | SettingScopes.User),
                new SettingDefinition(AppSettingNames.HoursToWaitUserToAcceptOfferOrRejectThem, "48", scopes: SettingScopes.Application | SettingScopes.All | SettingScopes.User),
                new SettingDefinition(AppSettingNames.HoursToMakeRequestOutOfPossible, "48", scopes: SettingScopes.Application | SettingScopes.All | SettingScopes.User),
                new SettingDefinition(AppSettingNames.DiscountPercentageIfUserCancelHisRequest, "30", scopes: SettingScopes.Application | SettingScopes.All | SettingScopes.User),
                new SettingDefinition(AppSettingNames.SenderHost, "smtp.gmail.com",scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SenderPort, "587",scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SenderEmail, "info@gomovaro.com",scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SenderPassword, "MNyea$h7CCzkFx!K",scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SenderEnableSsl, "true", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SenderUseDefaultCredentials, "false", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.Message, "Hello", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.MessageForResetPassword, "Reset Your Password", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SmsApiUrl, "", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SmsCountRetry, "5", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.EnFirstPartSmsMessage, "Your verification code is", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.EnSecondPartSmsMessage, "Please keep this code confidential.If you didn't request this code, please ignore this message.", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.ArFirstPartSmsMessage, "رمز التحقق الخاص بك هو", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.ArSecondPartSmsMessage, "يرجى الحفاظ على سرية هذا الرمز. في حال لم تقم بطلب الرمز، يُرجى تجاهل الرسالة", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SmsPassword, "0f3ffdeb98894831845260d8be813789", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.ServiceAccountSID, "VAd9272569db98ddc45787a8518949669c", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.SmsUserName, "AC1d633807c622236ac897f1c4d7119356", scopes: SettingScopes.Application | SettingScopes.All),
                new SettingDefinition(AppSettingNames.CommissionForBranchesWithoutCompany, "10", scopes: SettingScopes.Application | SettingScopes.All),
            };
        }
    }
}

using Abp.Authorization;
using Abp.Configuration;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReadIraq.Authorization;
using ReadIraq.Configuration.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReadIraq.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ReadIraqAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
        [HttpGet]
        [Tags("Enums")]
        [AbpAllowAnonymous]
        public List<EnumInfo> GetAllEnums()
        {
            var enumTypes = typeof(Enums.Enum).Assembly.GetTypes().Where(t => t.IsEnum && t.Namespace == ReadIraqConsts.EnumNameSpace);

            var result = new List<EnumInfo>();

            foreach (var enumType in enumTypes)
            {
                var underlyingType = System.Enum.GetUnderlyingType(enumType);
                var values = System.Enum.GetValues(enumType).Cast<object>().ToList();
                var names = System.Enum.GetNames(enumType).ToList();

                var enumInfo = new EnumInfo
                {
                    Name = enumType.Name,
                    Values = new List<EnumValue>()
                };

                for (int i = 0; i < values.Count; i++)
                {
                    enumInfo.Values.Add(new EnumValue
                    {
                        Name = names[i],
                        Value = Convert.ToByte(values[i])
                    });
                }

                result.Add(enumInfo);
            }

            return result;
        }
        /// <summary>
        /// kkkk
        /// </summary>
        /// <param name="input"></param>
        [AbpAuthorize(PermissionNames.SetEmailSetting)]
        public async Task SetEmailSetting(EmailSettingDto input)
        {


            if (input.SenderEmail != SettingManager.GetSettingValue(AppSettingNames.SenderEmail))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderEmail,
                        input.SenderEmail.ToString());

            if (input.SenderPassword != SettingManager.GetSettingValue(AppSettingNames.SenderPassword))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderPassword,
                      input.SenderPassword.ToString());

            if (input.SenderHost != SettingManager.GetSettingValue(AppSettingNames.SenderHost))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderHost,
                      input.SenderHost.ToString());

            if (input.Message != SettingManager.GetSettingValue(AppSettingNames.Message))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.Message,
                    input.Message.ToString());

            if (input.MessageForResetPassword != SettingManager.GetSettingValue(AppSettingNames.MessageForResetPassword))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.MessageForResetPassword,
                    input.MessageForResetPassword.ToString());

            if (input.SenderEnableSsl != SettingManager.GetSettingValue<bool>(AppSettingNames.SenderEnableSsl))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderEnableSsl,
                    input.SenderEnableSsl.ToString());

            if (input.SenderUseDefaultCredentials != SettingManager.GetSettingValue<bool>(AppSettingNames.SenderUseDefaultCredentials))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderUseDefaultCredentials,
                    input.SenderUseDefaultCredentials.ToString());

            if (input.SenderPort != SettingManager.GetSettingValue<int>(AppSettingNames.SenderPort))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SenderPort,
                    input.SenderPort.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        [AbpAuthorize(PermissionNames.GetEmailSetting)]
        public async Task<EmailSettingDto> GetEmailSetting()
        {

            var emailSettingDto = new EmailSettingDto()
            {
                SenderEmail = await SettingManager.GetSettingValueAsync(AppSettingNames.SenderEmail),
                SenderPassword = await SettingManager.GetSettingValueAsync(AppSettingNames.SenderPassword),
                SenderHost = await SettingManager.GetSettingValueAsync(AppSettingNames.SenderHost),
                SenderPort = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.SenderPort),
                SenderEnableSsl = await SettingManager.GetSettingValueAsync<bool>(AppSettingNames.SenderEnableSsl),
                SenderUseDefaultCredentials = await SettingManager.GetSettingValueAsync<bool>(AppSettingNames.SenderUseDefaultCredentials),
                Message = await SettingManager.GetSettingValueAsync(AppSettingNames.Message),
                MessageForResetPassword = await SettingManager.GetSettingValueAsync(AppSettingNames.MessageForResetPassword)
            };
            return emailSettingDto;
        }
        [AbpAuthorize(PermissionNames.SetSmsSetting)]
        public async Task SetSmsSetting(SmsSettingDto input)
        {
            if (input.ServiceAccountSID != SettingManager.GetSettingValue(AppSettingNames.ServiceAccountSID))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.ServiceAccountSID, input.ServiceAccountSID.ToString());

            if (input.SmsPassword != SettingManager.GetSettingValue(AppSettingNames.SmsPassword))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SmsPassword, input.SmsPassword.ToString());


            //if (input.SmsApiUrl != SettingManager.GetSettingValue(AppSettingNames.SmsApiUrl))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.SmsApiUrl,
            //        input.SmsApiUrl.ToString());

            //if (input.SmsCountRetry != SettingManager.GetSettingValue<int>(AppSettingNames.SmsCountRetry))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.SmsCountRetry,
            //        input.SmsCountRetry.ToString());

            if (input.SmsUserName != SettingManager.GetSettingValue(AppSettingNames.SmsUserName))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.SmsUserName, input.SmsUserName.ToString());

            //if (input.EnFirstPartSmsMessage != SettingManager.GetSettingValue(AppSettingNames.EnFirstPartSmsMessage))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.EnFirstPartSmsMessage,
            //        input.EnFirstPartSmsMessage.ToString());

            //if (input.EnSecondPartSmsMessage != SettingManager.GetSettingValue(AppSettingNames.EnSecondPartSmsMessage))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.EnSecondPartSmsMessage,
            //        input.EnSecondPartSmsMessage.ToString());

            //if (input.ArFirstPartSmsMessage != SettingManager.GetSettingValue(AppSettingNames.ArFirstPartSmsMessage))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.ArFirstPartSmsMessage,
            //        input.ArFirstPartSmsMessage.ToString());
            //if (input.ArSecondPartSmsMessage != SettingManager.GetSettingValue(AppSettingNames.ArSecondPartSmsMessage))
            //    SettingManager.ChangeSettingForApplication(AppSettingNames.ArSecondPartSmsMessage,
            //        input.ArSecondPartSmsMessage.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        [AbpAuthorize(PermissionNames.GetSmsSetting)]
        public async Task<SmsSettingDto> GetSmsSetting()
        {
            var smsSettingDto = new SmsSettingDto()
            {
                //SmsApiUrl = await SettingManager.GetSettingValueAsync(AppSettingNames.SmsApiUrl),
                //SmsCountRetry = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.SmsCountRetry),
                SmsPassword = await SettingManager.GetSettingValueAsync(AppSettingNames.SmsPassword),
                ServiceAccountSID = await SettingManager.GetSettingValueAsync(AppSettingNames.ServiceAccountSID),
                SmsUserName = await SettingManager.GetSettingValueAsync(AppSettingNames.SmsUserName),
                //EnFirstPartSmsMessage = await SettingManager.GetSettingValueAsync(AppSettingNames.EnFirstPartSmsMessage),
                //EnSecondPartSmsMessage = await SettingManager.GetSettingValueAsync(AppSettingNames.EnSecondPartSmsMessage),
                //ArFirstPartSmsMessage = await SettingManager.GetSettingValueAsync(AppSettingNames.ArFirstPartSmsMessage),
                //ArSecondPartSmsMessage = await SettingManager.GetSettingValueAsync(AppSettingNames.ArSecondPartSmsMessage),
            };
            return smsSettingDto;
        }
        /// <summary>
        /// Set Allowed Uploaded File Size in MegaByte 
        /// </summary>
        /// <param name="input"></param>
        [AbpAuthorize(PermissionNames.SetFileSizeSetting)]
        public async Task SetFileSizeSetting(FileSizeSettingDto input)
        {
            if (input.FileSize != SettingManager.GetSettingValue<double>(AppSettingNames.FileSize))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.FileSize,
                    input.FileSize.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        [AbpAuthorize(PermissionNames.GetFileSizeSetting)]
        public async Task<FileSizeSettingDto> GetFileSizeSetting()
        {
            var fileSettingDto = new FileSizeSettingDto()
            {
                FileSize = await SettingManager.GetSettingValueAsync<double>(AppSettingNames.FileSize),
            };
            return fileSettingDto;
        }
        [AbpAuthorize(PermissionNames.SetHoursInSystem)]
        public async Task SetHoursInSystem(HoursDto input)
        {
            if (input.HoursToWaitUser != SettingManager.GetSettingValue<int>(AppSettingNames.HoursToWaitUserToAcceptOfferOrRejectThem))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.HoursToWaitUserToAcceptOfferOrRejectThem,
                    input.HoursToWaitUser.ToString());
            if (input.HoursToConvertRequestToOutOfPossible != SettingManager.GetSettingValue<int>(AppSettingNames.HoursToMakeRequestOutOfPossible))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.HoursToMakeRequestOutOfPossible,
                    input.HoursToConvertRequestToOutOfPossible.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }
        [AbpAuthorize(PermissionNames.GetHoursInSystem)]
        public async Task<HoursDto> GetHoursInSystem()
        {
            var numberOfPointsDto = new HoursDto()
            {
                HoursToWaitUser = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.HoursToWaitUserToAcceptOfferOrRejectThem),
                HoursToConvertRequestToOutOfPossible = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.HoursToMakeRequestOutOfPossible)
            };
            return numberOfPointsDto;
        }
        [AbpAuthorize(PermissionNames.SetDiscountPercentage)]
        public async Task SetDiscountPercentage(DiscountPercentageDto input)
        {

            if (input.DiscountPercentageIfUserCancelHisRequest != SettingManager.GetSettingValue<int>(AppSettingNames.DiscountPercentageIfUserCancelHisRequest))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.DiscountPercentageIfUserCancelHisRequest,
                    input.DiscountPercentageIfUserCancelHisRequest.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        [AbpAuthorize(PermissionNames.GetDiscountPercentage)]
        public async Task<DiscountPercentageDto> GetDiscountPercentage()
        {
            return new DiscountPercentageDto() { DiscountPercentageIfUserCancelHisRequest = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.DiscountPercentageIfUserCancelHisRequest) };
        }
        [AbpAuthorize(PermissionNames.SetCommissionForBranchesWithoutCompany)]
        public async Task SetCommissionForBranchesWithoutCompany(CommissionGroupInputDto input)
        {

            if (input.CommissionForBranchesWithOutCompany != SettingManager.GetSettingValue<double>(AppSettingNames.CommissionForBranchesWithoutCompany))
                await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.CommissionForBranchesWithoutCompany,
                    input.CommissionForBranchesWithOutCompany.ToString());
            await UnitOfWorkManager.Current.SaveChangesAsync();

        }
        [AbpAuthorize(PermissionNames.GetCommissionForBranchesWithoutCompany)]
        public async Task<CommissionGroupInputDto> GetCommissionForBranchesWithoutCompany()
        {
            return new CommissionGroupInputDto() { CommissionForBranchesWithOutCompany = await SettingManager.GetSettingValueAsync<double>(AppSettingNames.CommissionForBranchesWithoutCompany) };
        }
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="input"></param>
        //public void SetImageSizeSetting(ImageSizeSettingDto input)
        //{
        //    if (input.ImageSize != SettingManager.GetSettingValue<int>(AppSettingNames.ImageSize))
        //        SettingManager.ChangeSettingForApplication(AppSettingNames.ImageSize,
        //            input.ImageSize.ToString());
        //}

        //public async Task<ImageSizeSettingDto> GetImageSizeSetting()
        //{
        //    var imageSettingDto = new ImageSizeSettingDto()
        //    {
        //        ImageSize = await SettingManager.GetSettingValueAsync<int>(AppSettingNames.ImageSize),
        //    };
        //    return imageSettingDto;
        //}
    }
}

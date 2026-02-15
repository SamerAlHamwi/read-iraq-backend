using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace ReadIraq.Authorization
{
    public class ReadIraqAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Users_List, L("UsersList"));
            context.CreatePermission(PermissionNames.Users_Create, L("UserCreate"));
            context.CreatePermission(PermissionNames.Users_Update, L("UserUpdate"));
            context.CreatePermission(PermissionNames.Users_Delete, L("UserDelete"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Roles_Create, L("RoleCreate"));
            context.CreatePermission(PermissionNames.Roles_Update, L("RoleUpdate"));
            context.CreatePermission(PermissionNames.Roles_Delete, L("RoleDelete"));
            context.CreatePermission(PermissionNames.Roles_List, L("RolesList"));
            context.CreatePermission(PermissionNames.Roles_GetAllPermission, L("RolesPermission"));
            context.CreatePermission(PermissionNames.Roles_Get, L("RoleGet"));
            context.CreatePermission(PermissionNames.Company_List, L("CompanyList"));
            context.CreatePermission(PermissionNames.Company_Create, L("CompanyCreate"));
            context.CreatePermission(PermissionNames.Company_Update, L("CompanyUpdate"));
            context.CreatePermission(PermissionNames.Company_Delete, L("CompanyDelete"));
            context.CreatePermission(PermissionNames.Company_ChangeStatus, L("CompanyChangeStatus"));
            context.CreatePermission(PermissionNames.Company_Including, L("CompanyIncluding"));
            context.CreatePermission(PermissionNames.Company_Get, L("CompanyGet"));
            context.CreatePermission(PermissionNames.CompanyBranch_List, L("CompanyBranchList"));
            context.CreatePermission(PermissionNames.CompanyBranch_Create, L("CompanyBranchCreate"));
            context.CreatePermission(PermissionNames.CompanyBranch_Update, L("CompanyBranchUpdate"));
            context.CreatePermission(PermissionNames.CompanyBranch_Delete, L("CompanyBranchDelete"));
            context.CreatePermission(PermissionNames.CompanyBranch_ChangeStatus, L("CompanyBranchChangeStatus"));
            context.CreatePermission(PermissionNames.CompanyBranch_Get, L("CompanyBranchGet"));
            context.CreatePermission(PermissionNames.Request_List, L("RequestList"));
            context.CreatePermission(PermissionNames.Request_Get, L("RequestGet"));
            context.CreatePermission(PermissionNames.Request_Create, L("CreateRequest"));
            context.CreatePermission(PermissionNames.Request_Update, L("UpdateRequest"));
            context.CreatePermission(PermissionNames.Request_Delete, L("DeleteRequest"));
            context.CreatePermission(PermissionNames.Request_ChangeStatus, L("ChangeRequestStatus"));
            context.CreatePermission(PermissionNames.Offer_List, L("OfferList"));
            context.CreatePermission(PermissionNames.Offer_Create, L("CreateOffer"));
            context.CreatePermission(PermissionNames.Offer_Update, L("UpdateOffer"));
            context.CreatePermission(PermissionNames.Offer_Delete, L("DeleteOffer"));
            context.CreatePermission(PermissionNames.Offer_ChangeStatus, L("ChangeOfferStatus"));
            context.CreatePermission(PermissionNames.Offer_Take, L("OfferTake"));
            context.CreatePermission(PermissionNames.City_FullControl, L("CityFullControl"));
            context.CreatePermission(PermissionNames.Region_FullControl, L("RegionFullControl"));
            context.CreatePermission(PermissionNames.Country_FullControl, L("CountryFullControl"));
            context.CreatePermission(PermissionNames.Grade_FullControl, L("GradeFullControl"));
            context.CreatePermission(PermissionNames.SourceType_FullControl, L("SourceTypeFullControl"));
            context.CreatePermission(PermissionNames.AttributeForSourceType_FullControl, L("AttributeForSourceTypeFullControl"));
            context.CreatePermission(PermissionNames.AttributeChoice_FullControl, L("AttributeChoiceFullControl"));
            context.CreatePermission(PermissionNames.Service_FullControl, L("ServiceFullControl"));
            context.CreatePermission(PermissionNames.SubService_FullControl, L("SubServiceFullControl"));
            context.CreatePermission(PermissionNames.Tool_FullControl, L("ToolFullControl"));
            context.CreatePermission(PermissionNames.PrivacyPolicy_FullControl, L("PrivacyPolicyFullControl"));
            context.CreatePermission(PermissionNames.Terms_FullControl, L("TermsFullControl"));
            context.CreatePermission(PermissionNames.FrequencyQuestion_FullControl, L("FrequencyQuestionFullControl"));
            context.CreatePermission(PermissionNames.RejectReason_FullControl, L("RejectReasonFullControl"));
            context.CreatePermission(PermissionNames.Points_FullControl, L("PointsFullControl"));
            context.CreatePermission(PermissionNames.ContactUs_FullControl, L("ContactUsFullControl"));
            context.CreatePermission(PermissionNames.CommissionGroup_FullControl, L("CommissionGroupFullControl"));
            context.CreatePermission(PermissionNames.ApkBuild_FullControl, L("ApkBuildFullControl"));
            context.CreatePermission(PermissionNames.Partner_FullControl, L("PartnerFullControl"));
            context.CreatePermission(PermissionNames.Broker_FullControl, L("BrokerFullControl"));
            context.CreatePermission(PermissionNames.GetCommissionForBranchesWithoutCompany, L("GetCommissionForBranchesWithoutCompany"));
            context.CreatePermission(PermissionNames.GetDiscountPercentage, L("GetDiscountPercentage"));
            context.CreatePermission(PermissionNames.GetEmailSetting, L("GetEmailSetting"));
            context.CreatePermission(PermissionNames.GetFileSizeSetting, L("GetFileSizeSetting"));
            context.CreatePermission(PermissionNames.GetHoursInSystem, L("GetHoursInSystem"));
            context.CreatePermission(PermissionNames.GetSmsSetting, L("GetSmsSetting"));
            context.CreatePermission(PermissionNames.SetCommissionForBranchesWithoutCompany, L("SetCommissionForBranchesWithoutCompany"));
            context.CreatePermission(PermissionNames.SetDiscountPercentage, L("SetDiscountPercentage"));
            context.CreatePermission(PermissionNames.SetEmailSetting, L("SetEmailSetting"));
            context.CreatePermission(PermissionNames.SetFileSizeSetting, L("SetFileSizeSetting"));
            context.CreatePermission(PermissionNames.SetHoursInSystem, L("SetHoursInSystem"));
            context.CreatePermission(PermissionNames.SetSmsSetting, L("SetSmsSetting"));



            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ReadIraqConsts.LocalizationSourceName);
        }
    }
}

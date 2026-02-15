namespace ReadIraq.Authorization
{
    public static class PermissionNames
    {
        public const string Pages_Tenants = "Pages.Tenants";

        public const string Pages_Users = "Pages.Users";
        public const string Pages_Users_Activation = "Pages.Users.Activation";

        public const string Pages_Roles = "Pages.Roles";

        public const string Users_List = Pages_Users + ".List";
        public const string Users_Create = Pages_Users + ".Create";
        public const string Users_Update = Pages_Users + ".Update";
        public const string Users_Delete = Pages_Users + ".Delete";

        public const string Roles_List = Pages_Roles + ".List";
        public const string Roles_Create = Pages_Roles + ".Create";
        public const string Roles_Update = Pages_Roles + ".Update";
        public const string Roles_Delete = Pages_Roles + ".Delete";
        public const string Roles_Get = Pages_Roles + ".Get";
        public const string Roles_GetAllPermission = Roles_List + ".Permission";

        public const string Request_List = "Request.List";
        public const string Request_Get = "Request.Get";
        public const string Request_Create = "Request.Create";
        public const string Request_Update = "Request.Update";
        public const string Request_Delete = "Request.Delete";
        public const string Request_ChangeStatus = "Request.ChangeStatus";

        public const string Company_List = "Company.List";
        public const string Company_Create = "Company.Create";
        public const string Company_Update = "Company.Update";
        public const string Company_Delete = "Company.Delete";
        public const string Company_Get = "Company.Get";
        public const string Company_ChangeStatus = "Company.ChangeStatues";
        public const string Company_Including = "Company.Including";

        public const string CompanyBranch_List = "CompanyBranch.List";
        public const string CompanyBranch_Create = "CompanyBranch.Create";
        public const string CompanyBranch_Update = "CompanyBranch.Update";
        public const string CompanyBranch_Delete = "CompanyBranch.Delete";
        public const string CompanyBranch_Get = "CompanyBranch.Get";
        public const string CompanyBranch_ChangeStatus = "CompanyBranch.ChangeStatues";

        public const string Offer_List = "Offer.List";
        public const string Offer_Create = "Offer.Create";
        public const string Offer_Update = "Offer.Update";
        public const string Offer_Delete = "Offer.Delete";
        public const string Offer_ChangeStatus = "Offer.ChangeStatus";
        public const string Offer_Take = "Offer.Take";

        public const string City_FullControl = "City.FullControl";
        public const string Region_FullControl = "Region.FullControl";
        public const string Country_FullControl = "Country.FullControl";
        public const string SourceType_FullControl = "SourceType.FullControl";
        public const string AttributeForSourceType_FullControl = "AttributeForSourceType.FullControl";
        public const string AttributeChoice_FullControl = "AttributeChoice.FullControl";
        public const string Service_FullControl = "Service.FullControl";
        public const string SubService_FullControl = "SubService.FullControl";
        public const string Tool_FullControl = "Tool.FullControl";
        public const string PrivacyPolicy_FullControl = "PrivacyPolicy.FullControl";
        public const string Terms_FullControl = "Terms.FullControl";
        public const string FrequencyQuestion_FullControl = "FrequencyQuestion.FullControl";
        public const string RejectReason_FullControl = "RejectReason.FullControl";
        public const string Points_FullControl = "Points.FullControl";
        public const string ContactUs_FullControl = "ContactUs.FullControl";
        public const string CommissionGroup_FullControl = "CommissionGroup.FullControl";
        public const string ApkBuild_FullControl = "ApkBuild.FullControl";
        public const string Partner_FullControl = "Partner.FullControl";
        public const string Broker_FullControl = "Broker.FullControl";

        public const string GetCommissionForBranchesWithoutCompany = "GetCommissionForBranchesWithoutCompany";
        public const string GetDiscountPercentage = "GetDiscountPercentage";
        public const string GetEmailSetting = "GetEmailSetting";
        public const string GetFileSizeSetting = "GetFileSizeSetting";
        public const string GetHoursInSystem = "GetHoursInSystem";
        public const string GetSmsSetting = "GetSmsSetting";
        public const string SetCommissionForBranchesWithoutCompany = "SetCommissionForBranchesWithoutCompany";
        public const string SetDiscountPercentage = "SetDiscountPercentage";
        public const string SetEmailSetting = "SetEmailSetting";
        public const string SetFileSizeSetting = "SetFileSizeSetting";
        public const string SetHoursInSystem = "SetHoursInSystem";
        public const string SetSmsSetting = "SetSmsSetting";






    }
}

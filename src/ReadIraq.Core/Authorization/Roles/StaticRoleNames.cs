namespace ReadIraq.Authorization.Roles
{
    public static class StaticRoleNames
    {
        public static class Host
        {
            public const string Admin = "Admin";
        }

        public static class Tenants
        {
            public const string Admin = "Admin";

            public const string BasicUser = "BasicUser";

            public const string CompanyUser = "CompanyUser";

            public const string CompanyBranchUser = "CompanyBranchUser";

            public const string CustomerService = "CustomerService";

        }
    }
}

using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly ReadIraqDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(ReadIraqDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }
            //var customerService = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.CustomerService);
            //if (customerService == null)
            //{
            //    customerService = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.CustomerService, StaticRoleNames.Tenants.CustomerService) { IsStatic = true }).Entity;
            //    _context.SaveChanges();
            //}
            var basicUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.BasicUser);
            if (basicUserRole == null)
            {
                basicUserRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.BasicUser, StaticRoleNames.Tenants.BasicUser) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }
            var companyUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == 2 && r.Name == StaticRoleNames.Tenants.CompanyUser);
            if (companyUserRole == null)
            {
                companyUserRole = _context.Roles.Add(new Role(2, StaticRoleNames.Tenants.CompanyUser, StaticRoleNames.Tenants.CompanyUser) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }
            var companyBranchUserRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == 2 && r.Name == StaticRoleNames.Tenants.CompanyBranchUser);
            if (companyBranchUserRole == null)
            {
                companyBranchUserRole = _context.Roles.Add(new Role(2, StaticRoleNames.Tenants.CompanyBranchUser, StaticRoleNames.Tenants.CompanyBranchUser) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new ReadIraqAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id
                    })
                );
                _context.SaveChanges();
            }
            CheckBasicUserRoles(basicUserRole);
            CheckCompanyUserRoles(companyUserRole);
            CheckCompanyBranchUserRoles(companyBranchUserRole);

            // Admin user

            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "x0220dIjB7");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;
                adminUser.Type = UserType.Admin;
                adminUser.PIN = new Random().Next(100000, 999999).ToString();
                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }

            var defaultGroup = _context.CommissionGroups.FirstOrDefault(x => x.IsDefault == true);
            if (defaultGroup is null)
            {
                defaultGroup = new Domain.CommissionGroups.CommissionGroup();
                defaultGroup.Name = 10;
                defaultGroup.IsDefault = true;

                _context.CommissionGroups.Add(defaultGroup);
                _context.SaveChanges();
            }
        }
        private void CheckBasicUserRoles(Role basicUserRole)
        {
            var basicUserPermissionInDB = _context
                .Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == basicUserRole.Id)
                .Select(x => x.Name)
                .ToList();

            var allBasicUserPermissions = new List<string>
            {
                PermissionNames.Pages_Users,
                PermissionNames.Request_Create,
                PermissionNames.Request_Delete,
                PermissionNames.Request_Update,
                PermissionNames.Request_List,
                PermissionNames.Request_Get,
                PermissionNames.Request_ChangeStatus,
                PermissionNames.Offer_Take,
                PermissionNames.Offer_List,
                PermissionNames.Offer_ChangeStatus,
                PermissionNames.CompanyBranch_List,
                PermissionNames.Company_List,
                PermissionNames.Company_Get,
                PermissionNames.CompanyBranch_Get
            };

            GrantPermissionToRole(
                role: basicUserRole,
                alreadyIncludedPermissions: basicUserPermissionInDB,
                actualPermissions: allBasicUserPermissions
            );
        }
        private void CheckCompanyBranchUserRoles(Role companyBranchUserRole)
        {
            var companyBranchRoleInDb = _context
                .Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == 2 && p.RoleId == companyBranchUserRole.Id)
                .Select(x => x.Name)
                .ToList();

            var allBasicUserPermissions = new List<string>
            {
                PermissionNames.Pages_Users,
                PermissionNames.CompanyBranch_Create,
                PermissionNames.CompanyBranch_Update,
                PermissionNames.CompanyBranch_Get,
                PermissionNames.CompanyBranch_Delete,
                PermissionNames.Request_List,
                PermissionNames.Request_Get,
                PermissionNames.Offer_Create,
                PermissionNames.Offer_Update,
                PermissionNames.Company_Get,
                PermissionNames.CompanyBranch_List
            };

            GrantPermissionToRole(
                role: companyBranchUserRole,
                alreadyIncludedPermissions: companyBranchRoleInDb,
                actualPermissions: allBasicUserPermissions,
                2
            );
        }
        private void CheckCompanyUserRoles(Role companyUserRole)
        {
            var companyRoleInDb = _context
                .Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == 2 && p.RoleId == companyUserRole.Id)
                .Select(x => x.Name)
                .ToList();

            var allBasicUserPermissions = new List<string>
            {
                PermissionNames.Pages_Users,
                PermissionNames.Company_Create,
                PermissionNames.Company_List,
                PermissionNames.Company_Update,
                PermissionNames.Company_Delete,
                PermissionNames.Company_Including,
                PermissionNames.Company_Get,
                PermissionNames.CompanyBranch_Create,
                PermissionNames.CompanyBranch_Update,
                PermissionNames.CompanyBranch_List,
                PermissionNames.CompanyBranch_Get,
                PermissionNames.CompanyBranch_Delete,
                PermissionNames.Request_List,
                PermissionNames.Request_Get,
                PermissionNames.Offer_Create,
                PermissionNames.Offer_Update,
            };

            GrantPermissionToRole(
                role: companyUserRole,
                alreadyIncludedPermissions: companyRoleInDb,
                actualPermissions: allBasicUserPermissions,
                2
            );
        }
        private void GrantPermissionToRole(Role role, List<string> alreadyIncludedPermissions, List<string> actualPermissions, int tenantId = 0)
        {
            var permissionsNotIncluded = PermissionFinder
                .GetAllPermissions(new ReadIraqAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) && !alreadyIncludedPermissions.Contains(p.Name) && actualPermissions.Contains(p.Name))
                .ToList();

            if (permissionsNotIncluded.Any())
            {
                _context.Permissions.AddRange(
                    permissionsNotIncluded.Select(permission => new RolePermissionSetting
                    {
                        RoleId = role.Id,
                        IsGranted = true,
                        TenantId = tenantId == 0 ? _tenantId : tenantId,
                        Name = permission.Name,
                    })
                );

                _context.SaveChanges();
            }
        }
    }
}

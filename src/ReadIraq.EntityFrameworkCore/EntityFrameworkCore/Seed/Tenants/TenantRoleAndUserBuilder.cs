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
            // SuperAdmin role
            var superAdminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.SuperAdmin);
            if (superAdminRole == null)
            {
                superAdminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.SuperAdmin, StaticRoleNames.Tenants.SuperAdmin) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Student role
            var studentRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Student);
            if (studentRole == null)
            {
                studentRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Student, StaticRoleNames.Tenants.Student) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Teacher role
            var teacherRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Teacher);
            if (teacherRole == null)
            {
                teacherRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Teacher, StaticRoleNames.Tenants.Teacher) { IsStatic = true }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to SuperAdmin role
            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == superAdminRole.Id)
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
                        RoleId = superAdminRole.Id
                    })
                );
                _context.SaveChanges();
            }

            CheckStudentRoles(studentRole);
            CheckTeacherRoles(teacherRole);

            // SuperAdmin user
            var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "x0220dIjB7");
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;
                adminUser.Type = UserType.SuperAdmin;
                adminUser.PIN = new Random().Next(100000, 999999).ToString();
                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign SuperAdmin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, superAdminRole.Id));
                _context.SaveChanges();
            }
        }

        private void CheckStudentRoles(Role studentRole)
        {
            var studentPermissionInDB = _context
                .Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == studentRole.Id)
                .Select(x => x.Name)
                .ToList();

            var allStudentPermissions = new List<string>
            {
                PermissionNames.Pages_Users,
                // Add student specific permissions here
            };

            GrantPermissionToRole(
                role: studentRole,
                alreadyIncludedPermissions: studentPermissionInDB,
                actualPermissions: allStudentPermissions
            );
        }

        private void CheckTeacherRoles(Role teacherRole)
        {
            var teacherPermissionInDB = _context
                .Permissions
                .IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == teacherRole.Id)
                .Select(x => x.Name)
                .ToList();

            var allTeacherPermissions = new List<string>
            {
                PermissionNames.Pages_Users,
                // Add teacher specific permissions here
            };

            GrantPermissionToRole(
                role: teacherRole,
                alreadyIncludedPermissions: teacherPermissionInDB,
                actualPermissions: allTeacherPermissions
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

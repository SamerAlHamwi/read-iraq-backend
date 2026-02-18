using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Roles;
using ReadIraq.Domain.Mediators;
using ReadIraq.Domain.Mediators.Mangers;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Users
{
    public class UserRegistrationManager : DomainService
    {
        public IAbpSession AbpSession { get; set; }

        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly MediatorManager _mediatorManager;
        private readonly IRepository<Mediator> _mediatorRepository;

        public UserRegistrationManager(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            MediatorManager mediatorManager,
            IRepository<Mediator> mediatorRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _tenantManager = tenantManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _mediatorManager = mediatorManager;
            _mediatorRepository = mediatorRepository;

            AbpSession = NullAbpSession.Instance;
        }

        public async Task<User> RegisterAsync(string name, string surname, string emailAddress, string userName, string plainPassword, bool isEmailConfirmed)
        {
            CheckForTenant();

            var tenant = await GetActiveTenantAsync();

            var user = new User
            {
                TenantId = tenant.Id,
                Name = name,
                Surname = surname,
                EmailAddress = emailAddress,
                IsActive = true,
                UserName = userName,
                PIN = await GenerateDefaultUniquePIN(),
                IsEmailConfirmed = isEmailConfirmed,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            {
                user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
            }

            await _userManager.InitializeOptionsAsync(tenant.Id);

            CheckErrors(await _userManager.CreateAsync(user, plainPassword));
            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        public async Task<User> RegisterAsync(string name, string surname, string emailAddress, string userName, string plainPassword, bool isEmailConfirmed, string phoneNumber, string dialCode, Enums.Enum.UserType userType, string registrationFullName, string mediatorCode = "")
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var tenant = await _tenantManager.FindByTenancyNameAsync("Default");

                var user = new User
                {
                    DialCode = dialCode,
                    PhoneNumber = phoneNumber,
                    TenantId = tenant.Id,
                    Name = name,
                    Surname = surname,
                    EmailAddress = emailAddress,
                    IsActive = true,
                    UserName = userName,
                    IsEmailConfirmed = isEmailConfirmed,
                    Roles = new List<UserRole>(),
                    Type = userType,
                    PIN = await GenerateDefaultUniquePIN(),
                    RegistrationFullName = registrationFullName,
                };

                if (!string.IsNullOrEmpty(mediatorCode))
                {
                    var mediator = (await _mediatorManager.GetAllMediatorsCodes()).FirstOrDefault(c => c.MediatorCode == mediatorCode);
                    if (mediator != null)
                    {
                        user.MediatorCode = mediator.MediatorCode;
                        await _mediatorRepository.UpdateAsync(mediator);
                    }
                    else
                    {
                        throw new UserFriendlyException(string.Format(Exceptions.MediatorCodeIsNotCorrect));
                    }
                }

                user.SetNormalizedNames();

                string roleName = StaticRoleNames.Tenants.Student;
                if (userType == UserType.SuperAdmin)
                {
                    roleName = StaticRoleNames.Tenants.SuperAdmin;
                }
                else if (userType == UserType.Teacher)
                {
                    roleName = StaticRoleNames.Tenants.Teacher;
                }

                foreach (var role in await _roleManager.Roles.Where(r => r.Name == roleName).ToListAsync())
                {
                    user.Roles.Add(new UserRole(tenant.Id, user.Id, role.Id));
                }

                await _userManager.InitializeOptionsAsync(tenant.Id);
                await CreateUserByTeneant(tenant.Id, user, plainPassword);
                return user;
            }
        }

        public async Task<User> RegisterUserAsync(string fullName, string dialCode, string phoneNumber, string password, int? gradeId, int? governorateId, UserType userType)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var tenant = await _tenantManager.FindByTenancyNameAsync("Default");
                if (tenant == null)
                {
                    throw new UserFriendlyException("Default tenant not found!");
                }

                var user = new User
                {
                    TenantId = tenant.Id,
                    Name = fullName,
                    Surname = fullName,
                    UserName = phoneNumber,
                    PhoneNumber = phoneNumber,
                    DialCode = dialCode,
                    RegistrationFullName = fullName,
                    GradeId = gradeId,
                    GovernorateId = governorateId,
                    Type = userType,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    PIN = await GenerateDefaultUniquePIN(),
                    EmailAddress = await GenerateRandomEmail(),
                    Roles = new List<UserRole>()
                };

                user.SetNormalizedNames();

                string roleName = StaticRoleNames.Tenants.Student;
                if (userType == UserType.SuperAdmin)
                {
                    roleName = StaticRoleNames.Tenants.SuperAdmin;
                }
                else if (userType == UserType.Teacher)
                {
                    roleName = StaticRoleNames.Tenants.Teacher;
                }

                var roles = await _roleManager.Roles.Where(r => r.Name == roleName).ToListAsync();
                foreach (var role in roles)
                {
                    user.Roles.Add(new UserRole(tenant.Id, user.Id, role.Id));
                }

                await _userManager.InitializeOptionsAsync(tenant.Id);
                await CreateUserByTeneant(tenant.Id, user, password);

                return user;
            }
        }

        private async Task CreateUserByTeneant(int tenantId, User userForCreate, string password)
        {
            using (UnitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
                {
                    CheckErrors(await _userManager.CreateAsync(userForCreate, password));
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
        }

        private async Task<string> GenerateRandomEmail()
        {
            Random random = new Random();
            string Suffix = "@EntityFrameWorkCore.net";
            string generatedEmailAddress;
            do
            {
                int randomNumber = random.Next(1, 100000);
                generatedEmailAddress = $"{randomNumber:D5}{Suffix}";
            }
            while (await _userManager.Users.AnyAsync(x => x.EmailAddress == generatedEmailAddress));
            return generatedEmailAddress;
        }

        private void CheckForTenant()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private async Task<Tenant> GetActiveTenantAsync()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return await GetActiveTenantAsync(AbpSession.TenantId.Value);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await _tenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<string> GenerateDefaultUniquePIN()
        {
            while (true)
            {
                int randomValue = new Random().Next(100000, 999999);
                string generatedPIN = randomValue.ToString();

                bool isUnique = !await _userManager.Users.AnyAsync(u => u.PIN == generatedPIN);

                if (isUnique)
                {
                    return generatedPIN;
                }
            }
        }
    }
}

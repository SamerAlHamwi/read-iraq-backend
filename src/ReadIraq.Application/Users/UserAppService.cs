using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.AskForHelps;
using ReadIraq.Domain.AskForHelps.Dto;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.NotificationSender;
using ReadIraq.Roles.Dto;
using ReadIraq.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UpdateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly INotificationSender _notificationSender;
        private readonly IRepository<AskForHelp> _askForHelpRepository;
        private readonly UserRegistrationManager _userRegistrationManager;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IAbpSession abpSession,
            LogInManager logInManager,
            INotificationSender notificationSender,
            UserRegistrationManager userRegistrationManager,
            IRepository<AskForHelp> askForHelpRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _notificationSender = notificationSender;
            _askForHelpRepository = askForHelpRepository;
            _userRegistrationManager = userRegistrationManager;
        }
        [AbpAuthorize(PermissionNames.Users_List)]
        public override async Task<PagedResultDto<UserDto>> GetAllAsync(PagedUserResultRequestDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                return await base.GetAllAsync(input);
            }
        }
        public async Task SetRolesForAllUsers(UserType user, string roleName)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var users = await _userManager.Users.IgnoreQueryFilters().Where(x => x.Type == user && x.IsDeleted == false).ToListAsync();
                foreach (var hadmd in users)
                {
                    CheckErrors(await _userManager.SetRolesAsync(hadmd, new[] { roleName }));
                }
            }
        }
        [AbpAuthorize(PermissionNames.Users_Create)]
        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                try
                {
                    CheckCreatePermission();
                    var currentUser = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
                    var isNotSuperAdmin = currentUser.Type != UserType.SuperAdmin;
                    if ((isNotSuperAdmin && input.Type == UserType.SuperAdmin) || (isNotSuperAdmin && input.RoleNames.Any(x => x.Contains(StaticRoleNames.Tenants.SuperAdmin))))
                        throw new UserFriendlyException(Exceptions.YouCannotDoThisAction);

                    var user = ObjectMapper.Map<User>(input);
                    user.RegistrationFullName = user.Name + " " + user.Surname;
                    user.TenantId = AbpSession.TenantId;
                    user.IsEmailConfirmed = true;
                    user.Type = input.Type;
                    user.PIN = await _userRegistrationManager.GenerateDefaultUniquePIN();
                    await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                    CheckErrors(await _userManager.CreateAsync(user, input.Password));

                    if (input.RoleNames != null)
                    {
                        CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
                    }

                    CurrentUnitOfWork.SaveChanges();

                    return MapToEntityDto(user);
                }
                catch (Exception se)
                {
                    throw;
                }
            }
        }
        [AbpAuthorize(PermissionNames.Users_Update)]
        public override async Task<UserDto> UpdateAsync(UpdateUserDto input)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                try
                {
                    CheckUpdatePermission();

                    var user = await _userManager.GetUserByIdAsync(input.Id);

                    user.RegistrationFullName = user.Name + " " + user.Surname;
                    user.TenantId = AbpSession.TenantId;
                    user.IsEmailConfirmed = true;
                    user.Type = input.Type;
                    user.IsActive = input.IsActive;
                    user.UserName = input.UserName;
                    user.Surname = input.Surname;
                    user.Name = input.Name;
                    user.EmailAddress = input.EmailAddress;
                    user.MediatorCode = input.MediatorCode;

                    CheckErrors(await _userManager.UpdateAsync(user));

                    if (input.RoleNames != null)
                    {
                        CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
                    }
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    return MapToEntityDto(user);
                }
                catch (Exception ex) { throw; }
            }
        }
        [AbpAuthorize(PermissionNames.Users_Delete)]
        public override async Task DeleteAsync(EntityDto<long> input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userManager.GetUserByIdAsync(input.Id);
                await _userManager.DeleteAsync(user);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task Activate(EntityDto<long> user)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = true;
            });
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
        public async Task DeActivate(EntityDto<long> user)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                await Repository.UpdateAsync(user.Id, async (entity) =>
            {
                entity.IsActive = false;
            });
            }
        }
        [AbpAuthorize(PermissionNames.Roles_List)]
        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
            }
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UpdateUserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            int[] roleIds = new List<int>().ToArray();
            if (user.Roles is not null)
            {
                roleIds = user.Roles.Select(x => x.RoleId).ToArray();
            }
            var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();

            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
        {
            var data = Repository.GetAllIncluding(x => x.Roles)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword) || x.PIN.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive)
                    .WhereIf(input.UserType.HasValue, x => x.Type == input.UserType)
                    .WhereIf(!string.IsNullOrEmpty(input.MediatorCode), x => x.MediatorCode.Contains(input.MediatorCode));

            return data;
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
        {
            return query.OrderByDescending(x => x.CreationTime);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public async Task<bool> ChangePassword(ChangePasswordDto input)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

                var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
                if (user == null)
                {
                    throw new Exception("There is no current user!");
                }

                if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
                {
                    CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
                }
                else
                {
                    CheckErrors(IdentityResult.Failed(new IdentityError
                    {
                        Description = "Incorrect password."
                    }));
                }

                return true;
            }
        }
        /// <summary>
        /// Get FCM Token for current user, if exists
        /// </summary>
        public async Task<string> GetCurrentFcmTokenAsync()
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await GetEntityByIdAsync(AbpSession.UserId.Value);

                return user.FcmToken;
            }
        }
        /// <summary>
        /// Set or clear FCM Token for current user
        /// </summary>
        public async Task SetCurrentFcmTokenAsync(string input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await GetEntityByIdAsync(AbpSession.UserId.Value);

                // Update user fcm token
                user.FcmToken = input;
                await Repository.UpdateAsync(user);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                if (_abpSession.UserId == null)
                {
                    throw new UserFriendlyException("Please log in before attempting to reset password.");
                }

                var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
                var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
                if (loginAsync.Result != AbpLoginResultType.Success)
                {
                    throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
                }

                if (currentUser.IsDeleted || !currentUser.IsActive)
                {
                    return false;
                }

                var roles = await _userManager.GetRolesAsync(currentUser);
                if (!roles.Contains(StaticRoleNames.Tenants.SuperAdmin))
                {
                    throw new UserFriendlyException("Only administrators may reset passwords.");
                }

                var user = await _userManager.GetUserByIdAsync(input.UserId);
                if (user != null)
                {
                    user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                return true;
            }
        }
        public async Task<ResuktComparePinPhoneNumberDto> CheckIfUserOwnesPinByPhoneNumber(ComparePinPhoneNumberDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var result = new ResuktComparePinPhoneNumberDto();
                var user = await _userManager.Users.Where(x => x.PhoneNumber == input.PhoneNumber && x.PIN == input.PIN).FirstOrDefaultAsync();
                if (user is null) { result.IsOwner = false; return result; }
                result.UserId = user.Id;
                result.IsOwner = true;
                return result;
            }
        }
        [HttpGet]
        public async Task<CheckIsBrokerDto> CheckIfUserIsBroker()
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
                if (user is null) { throw new UserFriendlyException(Exceptions.ObjectWasNotFound, Tokens.User); }
                var reult = new CheckIsBrokerDto();
                reult.IsBroker = false;
                return reult;
            }
        }
        [Tags("AskForHelp")]
        [AbpAuthorize]
        public async Task<bool> AskForHelp(string message)
        {
            var askForHelp = new AskForHelp { UserId = AbpSession.UserId.Value, Statues = AskForHelpStatues.Waiting, Message = message };
            await _askForHelpRepository.InsertAsync(askForHelp);
            var userIds = await _userManager.Users.Where(x => x.Type == UserType.SuperAdmin).Select(x => x.Id).ToListAsync();
            await _notificationSender.SendNotificationForCostumerServiceForAskHelp(userIds, AbpSession.UserId.Value);
            return true;
        }
        [Tags("AskForHelp")]
        public async Task<IList<LiteAskForHelpDto>> GetAllAskForHelps(PagedAskForHelpResultRequestDto input)
        {
            var data = _askForHelpRepository.GetAllIncluding(x => x.User);
            if (input.Statues.HasValue)
                data = data.Where(x => x.Statues == input.Statues);

            var result = ObjectMapper.Map(data.ToList(), new List<LiteAskForHelpDto>());
            return result.OrderByDescending(x => x.CreationTime).ToList();
        }
        [Tags("AskForHelp")]
        public async Task<bool> ConfirmFollowedForAskingForHelp(int id)
        {
            var askForHelp = await _askForHelpRepository.GetAsync(id);
            if (askForHelp == null)
                throw new UserFriendlyException(404, Exceptions.ObjectWasNotFound, Tokens.Entity);
            askForHelp.Statues = AskForHelpStatues.Followed;
            await _askForHelpRepository.UpdateAsync(askForHelp);
            return true;
        }

        [AbpAuthorize]
        [Tags("Dashboard")]
        public async Task<UsersStatisticalNumbersDto> GetStatisticalNumbers(UsersStatisticalInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                if (!input.Year.HasValue) input.Year = DateTime.Now.Year;
                var result = await _userManager.Users
                  .GroupBy(u => 1)
                  .Select(users => new UsersStatisticalNumbersDto
                  {
                      TotalCount = users.Count(),
                      SuperAdmins = users.Where(u => u.Type == UserType.SuperAdmin && u.IsDeleted == false).Count(),
                      Students = users.Where(u => u.Type == UserType.Student && u.IsDeleted == false).Count(),
                      Teachers = users.Where(u => u.Type == UserType.Teacher && u.IsDeleted == false).Count(),
                      ActiveUsers = users.Where(u => u.IsActive == true).Count(),
                      DeActiveUsers = users.Where(u => u.IsActive == false && u.IsDeleted == false).Count(),
                      ChartPoints = users.Where(u => u.CreationTime.Year == input.Year.Value && u.Type == UserType.Student)
                                   .GroupBy(u => new { Month = u.CreationTime.Month })
                                   .Select(group => new InfoForUserChart
                                   {
                                       Month = group.Key.Month,
                                       UserCount = group.Count()
                                   })
                                   .OrderBy(result => result.Month)
                                   .ToList()
                  }).FirstOrDefaultAsync();

                if (result is null) return new UsersStatisticalNumbersDto();
                return result;
            }
        }
    }
}

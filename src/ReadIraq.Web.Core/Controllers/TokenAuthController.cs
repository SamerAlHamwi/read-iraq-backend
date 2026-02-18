using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.UI;
using ReadIraq.Domain.ChangedPhoneNumber;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authentication.External;
using ReadIraq.Authentication.JwtBearer;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Mediators.Mangers;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.Models.TokenAuth;
using ReadIraq.MultiTenancy;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;
using Microsoft.IdentityModel.Tokens;

namespace ReadIraq.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TokenAuthController : ReadIraqControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly ReadIraq.Authentication.JwtBearer.TokenAuthConfiguration _configuration;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IUserVerficationCodeManager _userVerficationCodeManager;
        private readonly IRegisterdPhoneNumberManager _registerdPhoneNumberManager;
        private readonly UserManager _userManager;
        private readonly IRepository<ChangedPhoneNumberForUser> _changedPhoneNumberForUserRepository;
        private readonly IMediatorManager _mediatorManager;
        private readonly TenantManager _tenantManager;
        private readonly IRepository<ReadIraq.Authorization.Users.User, long> _userRepository;
        private readonly RoleManager _roleManager;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            ReadIraq.Authentication.JwtBearer.TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            IUserVerficationCodeManager userVerficationCodeManager,
            IRegisterdPhoneNumberManager registerdPhoneNumberManager,
            UserManager userManager,
            IRepository<ChangedPhoneNumberForUser> changedPhoneNumberForUserRepository,
            IMediatorManager mediatorManager,
            TenantManager tenantManager,
            IRepository<ReadIraq.Authorization.Users.User, long> userRepository,
            RoleManager roleManager)
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _userVerficationCodeManager = userVerficationCodeManager;
            _registerdPhoneNumberManager = registerdPhoneNumberManager;
            _userManager = userManager;
            _changedPhoneNumberForUserRepository = changedPhoneNumberForUserRepository;
            _mediatorManager = mediatorManager;
            _tenantManager = tenantManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var tenant = await _tenantManager.FindByTenancyNameAsync("Default");
            var loginResult = await GetLoginResultAsync(
                   model.UserNameOrEmailAddress,
                   model.Password,
                   tenant.Name
               );

            var permissions = new List<string>();
            var roleNames = await _userManager.GetRolesAsync(loginResult.User);

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                var grantedPermissions = await _roleManager.GetGrantedPermissionsAsync(role);
                permissions.AddRange(grantedPermissions.Select(x => x.Name).ToList());
            }

            try
            {
                var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

                var result = new AuthenticateResultModel
                {
                    AccessToken = accessToken,
                    EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    UserId = loginResult.User.Id,
                    UserType = loginResult.User.Type,
                    Language = await SettingManager.GetSettingValueForUserAsync(
                        LocalizationSettingNames.DefaultLanguage,
                        loginResult.Tenant.Id,
                        loginResult.User.Id),
                    Permissions = permissions.Distinct().ToList(),
                };

                return result;
            }
            catch (Exception ex) { throw; }
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        var accessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity));

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        private async Task<ReadIraq.Authorization.Users.User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                ReadIraq.Authorization.Users.User.CreateRandomPassword(),
                false
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, ReadIraq.Authorization.Users.User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken);
        }

        [HttpPost]
        public async Task<VerifyLoginByPhoneNumberOutput> CreateAccountAfterSignUpAsync([FromBody] VerifySignUpByPhoneNumberInput input)
        {
            var registerdUser = await _registerdPhoneNumberManager.GetRegisteredPhoneNumberAsync(input.DialCode, input.PhoneNumber);
            if (registerdUser is not null)
            {
                if (!await _registerdPhoneNumberManager.CheckPhoneNumberIsVerifiedAsync(input.DialCode, input.PhoneNumber))
                {
                    throw new UserFriendlyException(string.Format(Exceptions.YourPhoneNumberIsntVerified));
                }
                if (string.IsNullOrEmpty(input.Email))
                {
                    Random random = new Random();
                    int randomNumber = random.Next(100, 100000);
                    input.Email = input.FullName + randomNumber.ToString() + "@EntityFrameWorkCore.net";
                }

                var user = await _userRegistrationManager.RegisterAsync(string.Empty,
                  string.Empty,
                  input.Email,
                  input.PhoneNumber,
                  input.Password,
                  true,
                  input.PhoneNumber,
                  input.DialCode,
                  input.UserType,
                  input.FullName,
                  input.MediatorCode);

                AbpLoginResult<Tenant, ReadIraq.Authorization.Users.User> loginResult = await GetLoginResultAsync(
                  input.PhoneNumber,
                  input.Password,
                  "Default");

                await _userVerficationCodeManager.AddUserVerficationCodeAsync(
                    new UserVerficationCode
                    {
                        ConfirmationCodeType = Enums.Enum.ConfirmationCodeType.ConfirmPhoneNumber,
                        UserId = user.Id,
                        VerficationCode = registerdUser.VerficationCode
                    });

                return new VerifyLoginByPhoneNumberOutput
                {
                    AccessToken = CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                    UserId = user.Id,
                    UserName = user.UserName,
                    ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                    UserType = loginResult.User.Type,
                };
            }
            else
                throw new UserFriendlyException(string.Format(Exceptions.SignUpNotComplete));
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> VerifySignInWithPhoneNumberAsync([FromBody] VerifyLoginByPhoneNumberInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var registerdUser = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == input.PhoneNumber && x.DialCode == input.DialCode);

                if (registerdUser is not null)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotCorrect));
                }
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.User));
            }
        }

        [HttpPut]
        public async Task<bool> VerifyUserToSetNewPassword([FromBody] VerifyResetPasswordForUserInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var registerdUser = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == input.PhoneNumber && x.DialCode == input.DialCode);
                if (registerdUser is not null)
                {
                    CheckErrors(await _userManager.ChangePasswordAsync(registerdUser, input.NewPassword));
                    return true;
                }
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.User));
            }
        }

        [HttpPut]
        public async Task<bool> VerifyUserToSetNewPasswordByEmail([FromBody] VerifyResetPasswordForUserUsingEmailInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var registerdUser = await _userManager.Users.FirstOrDefaultAsync(x => x.EmailAddress == input.EmailAddress);
                if (registerdUser is not null)
                {
                    if (!registerdUser.IsEmailConfirmed)
                        throw new UserFriendlyException(Exceptions.YourEmailIsNotConfirmed);
                    CheckErrors(await _userManager.ChangePasswordAsync(registerdUser, input.NewPassword));
                    return true;
                }
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.User));
            }
        }

        [HttpPost]
        public async Task<bool> VerifyChangePhoneNumberAsync([FromBody] VerifyChangePhoneNumberInput input)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var newPhoneNumberForUser = await _changedPhoneNumberForUserRepository.GetAll().Where(x => x.NewPhoneNumber == input.PhoneNumber && x.NewDialCode == input.DialCode).FirstOrDefaultAsync();
                if (newPhoneNumberForUser is not null)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotCorrect));
                }
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PhoneNumber));
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials

            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}

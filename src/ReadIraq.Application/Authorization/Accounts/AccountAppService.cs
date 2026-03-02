using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authentication.JwtBearer;
using ReadIraq.Authorization.Accounts.Dto;
using ReadIraq.Authorization.Users;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.Localization.SourceFiles;
using ReadIraq.Domain.Teachers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Accounts
{
    public class AccountAppService : ReadIraqAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;
        private readonly LogInManager _logInManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRegisterdPhoneNumberManager _registerdPhoneNumberManager;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Grade, int> _gradeRepository;
        private readonly IRepository<City, int> _cityRepository;
        private readonly IRepository<UserPreferredSubject, Guid> _userPreferredSubjectRepository;
        private readonly IRepository<UserPreferredTeacher, Guid> _userPreferredTeacherRepository;
        private readonly ITokenAuthManager _tokenAuthManager;
        private readonly UserClaimsPrincipalFactory _claimsPrincipalFactory;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            UserManager userManager,
            LogInManager logInManager,
            IPasswordHasher<User> passwordHasher,
            IRegisterdPhoneNumberManager registerdPhoneNumberManager,
            IAttachmentManager attachmentManager,
            IRepository<User, long> userRepository,
            IRepository<Grade, int> gradeRepository,
            IRepository<City, int> cityRepository,
            IRepository<UserPreferredSubject, Guid> userPreferredSubjectRepository,
            IRepository<UserPreferredTeacher, Guid> userPreferredTeacherRepository,
            ITokenAuthManager tokenAuthManager,
            UserClaimsPrincipalFactory claimsPrincipalFactory
            )
        {
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
            _logInManager = logInManager;
            _passwordHasher = passwordHasher;
            _registerdPhoneNumberManager = registerdPhoneNumberManager;
            _attachmentManager = attachmentManager;
            _userRepository = userRepository;
            _gradeRepository = gradeRepository;
            _cityRepository = cityRepository;
            _userPreferredSubjectRepository = userPreferredSubjectRepository;
            _userPreferredTeacherRepository = userPreferredTeacherRepository;
            _tokenAuthManager = tokenAuthManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<RegisterUserOutput> RegisterUser(RegisterUserInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var dialCode = input.DialCode.Trim();
                var phone = input.Phone.Trim();

                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone && u.DialCode == dialCode);
                if (existingUser != null)
                {
                    throw new UserFriendlyException(409, L(nameof(Exceptions.ObjectIsAlreadyExist), L(nameof(Tokens.PhoneNumber))));
                }

                if (input.GradeId.HasValue && !await _gradeRepository.GetAll().AnyAsync(x => x.Id == input.GradeId.Value))
                {
                    throw new UserFriendlyException(400, L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.Grade))));
                }

                if (input.GovernorateId.HasValue && !await _cityRepository.GetAll().AnyAsync(x => x.Id == input.GovernorateId.Value))
                {
                    throw new UserFriendlyException(400, L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.City))));
                }

                var user = await _userRegistrationManager.RegisterUserAsync(
                    input.FullName,
                    dialCode,
                    phone,
                    input.Password,
                    input.GradeId,
                    input.GovernorateId,
                    input.UserType
                );

                var registeredPhone = await _registerdPhoneNumberManager.UpdateVerificationCodeAsync(dialCode, phone);
                await UnitOfWorkManager.Current.SaveChangesAsync();

                return new RegisterUserOutput
                {
                    UserId = user.Id.ToString(),
                    Phone = $"{user.DialCode}{user.PhoneNumber}",
                    OtpSent = true,
                    VerificationCode = registeredPhone.VerficationCode,
                    Message = $"OTP {registeredPhone.VerficationCode} sent to phone"
                };
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<VerifySignUpOutput> VerifySignUpWithPhoneNumber(VerifySignUpInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var dialCode = input.DialCode.Trim();
                var phone = input.Phone.Trim();

                if (!await _registerdPhoneNumberManager.CheckVerificationCodeIsValidAsync(dialCode, phone, input.Otp))
                {
                    throw new UserFriendlyException(L(nameof(Exceptions.VerificationCodeIsnotCorrect)));
                }

                var user = await _userManager.Users
                    .Include(u => u.Grade)
                    .Include(u => u.Governorate).ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.PhoneNumber == phone && u.DialCode == dialCode);

                if (user == null)
                {
                    throw new UserFriendlyException(L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.User))));
                }

                user.IsPhoneNumberConfirmed = true;
                user.IsActive = true;
                await _userManager.UpdateAsync(user);

                await _registerdPhoneNumberManager.VerifiedPhoneNumberAsync(dialCode, phone);

                var principal = await _claimsPrincipalFactory.CreateAsync(user);
                var accessToken = _tokenAuthManager.CreateAccessToken(principal.Identity as ClaimsIdentity);

                return new VerifySignUpOutput
                {
                    User = ObjectMapper.Map<UserDetailDto>(user),
                    Token = accessToken
                };
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<ResendCodeOutput> ResendVerificationCode(ResendCodeInput input)
        {
            var registeredPhone = await _registerdPhoneNumberManager.UpdateVerificationCodeAsync(input.DialCode.Trim(), input.Phone.Trim());
            return new ResendCodeOutput
            {
                OtpSent = true,
                Message = $"OTP {registeredPhone.VerficationCode} sent to phone"
            };
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<SignInOutput> SignInWithPhoneNumber(SignInInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var phone = input.Phone.Trim();
                var loginResult = await _logInManager.LoginAsync(phone, input.Password, "Default");

                if (loginResult.Result != AbpLoginResultType.Success)
                {
                    throw new UserFriendlyException(L("InvalidUserNameOrPassword"));
                }

                var user = await _userManager.Users
                    .Include(u => u.Grade)
                    .Include(u => u.Governorate).ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == loginResult.User.Id);

                var accessToken = _tokenAuthManager.CreateAccessToken(loginResult.Identity);

                return new SignInOutput
                {
                    Token = accessToken,
                    RefreshToken = string.Empty,
                    User = ObjectMapper.Map<UserDetailDto>(user)
                };
            }
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task LogOut()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            user.FcmToken = null;
            await _userManager.UpdateAsync(user);
        }

        [AbpAuthorize]
        [HttpGet]
        public async Task<UserDetailDto> GetProfileInfo()
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userManager.Users
                    .Include(u => u.Grade)
                    .Include(u => u.Governorate).ThenInclude(c => c.Translations)
                    .FirstOrDefaultAsync(u => u.Id == AbpSession.GetUserId());

                var result = ObjectMapper.Map<UserDetailDto>(user);

                var attachment = await _attachmentManager.GetElementByRefAsync(user.Id.ToString(), AttachmentRefType.Profile);
                if (attachment != null)
                {
                    result.ProfilePhoto = new LiteAttachmentDto
                    {
                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                    };
                }
                return result;
            }
        }

        [AbpAuthorize]
        [HttpPut]
        public async Task UpdateProfile(UpdateProfileDto input)
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            user.Name = input.Name;
            user.EmailAddress = input.EmailAddress;
            user.RegistrationFullName = input.Name;
            user.GradeId = input.GradeId;
            user.GovernorateId = input.GovernorateId;

            if (input.ProfilePhoto > 0)
            {
                await _attachmentManager.CheckAndUpdateRefIdAsync((long)input.ProfilePhoto, AttachmentRefType.Profile, user.Id.ToString());
            }
            else if (input.ProfilePhoto == 0)
            {
                var oldAttachment = await _attachmentManager.GetElementByRefAsync(user.Id.ToString(), AttachmentRefType.Profile);
                if (oldAttachment != null) await _attachmentManager.DeleteRefIdAsync(oldAttachment);
            }

            await _userManager.UpdateAsync(user);
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task AddOrEditUserProfilePhoto(AddUserProfilePhotoDto input)
        {
            var oldAttachment = await _attachmentManager.GetElementByRefAsync(AbpSession.GetUserId().ToString(), AttachmentRefType.Profile);
            if (oldAttachment != null)
            {
                await _attachmentManager.DeleteRefIdAsync(oldAttachment);
            }
            var attachment = await _attachmentManager.GetByIdAsync(input.PhotoId);
            await _attachmentManager.UpdateRefIdAsync(attachment, AbpSession.GetUserId().ToString());
        }

        [AbpAuthorize]
        public async Task DeleteAccount()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            if (user.Type == UserType.SuperAdmin)
            {
                throw new UserFriendlyException(403, L(nameof(Exceptions.YouCannotDoThisAction)));
            }
            await _userManager.DeleteAsync(user);
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task SetUserPreferredSubjects(SetPreferredSubjectsInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var existingPreferredSubjects = await _userPreferredSubjectRepository.GetAllListAsync(x => x.UserId == input.UserId);
                foreach (var item in existingPreferredSubjects)
                {
                    await _userPreferredSubjectRepository.DeleteAsync(item);
                }

                foreach (var subjectId in input.SubjectIds)
                {
                    await _userPreferredSubjectRepository.InsertAsync(new UserPreferredSubject
                    {
                        UserId = input.UserId,
                        SubjectId = subjectId,
                        ProgressPercent = 0,
                        CreationTime = DateTime.UtcNow
                    });
                }
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task SetUserPreferredTeacherSubjects(SetPreferredTeacherSubjectsInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var existingPreferredTeachers = await _userPreferredTeacherRepository.GetAllListAsync(x => x.UserId == input.UserId);
                foreach (var item in existingPreferredTeachers)
                {
                    await _userPreferredTeacherRepository.DeleteAsync(item);
                }

                foreach (var item in input.TeacherSubjects)
                {
                    foreach (var teacherId in item.TeachersId)
                    {
                        await _userPreferredTeacherRepository.InsertAsync(new UserPreferredTeacher
                        {
                            UserId = input.UserId,
                            SubjectId = item.SubjectId,
                            TeacherProfileId = teacherId
                        });
                    }
                }
            }
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task ChangePassword(ChangePasswordInput input)
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());
            CheckErrors(await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword));
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task ForgotPassword(ForgotPasswordInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == input.Phone && u.DialCode == input.DialCode);
                if (user == null)
                {
                    throw new UserFriendlyException(L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.User))));
                }

                await _registerdPhoneNumberManager.UpdateVerificationCodeAsync(input.DialCode, input.Phone);
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task VerifyForgotPassword(VerifyForgotPasswordInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                if (!await _registerdPhoneNumberManager.CheckVerificationCodeIsValidAsync(input.DialCode, input.Phone, input.Otp))
                {
                    throw new UserFriendlyException(L(nameof(Exceptions.VerificationCodeIsnotCorrect)));
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == input.Phone && u.DialCode == input.DialCode);
                if (user == null)
                {
                    throw new UserFriendlyException(L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.User))));
                }

                // In ABP/Identity, to reset password without token, we can use RemovePassword and AddPassword
                // or just ChangePasswordAsync if we have a way to bypass old password.
                // Here we use a simpler way if allowed:
                user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
                await _userManager.UpdateAsync(user);

                await _registerdPhoneNumberManager.VerifiedPhoneNumberAsync(input.DialCode, input.Phone);
            }
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task RequestChangePhoneNumber(ChangePhoneNumberDto input)
        {
            // Send OTP to NEW number
            await _registerdPhoneNumberManager.UpdateVerificationCodeAsync(input.DialCode, input.PhoneNumber);
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task VerifyChangePhoneNumber(VerifyChangePhoneNumberDto input)
        {
            if (!await _registerdPhoneNumberManager.CheckVerificationCodeIsValidAsync(input.NewDialCode, input.NewPhone, input.Otp))
            {
                throw new UserFriendlyException(L(nameof(Exceptions.VerificationCodeIsnotCorrect)));
            }

            var user = await _userManager.GetUserByIdAsync(AbpSession.GetUserId());

            // Check if new phone is already used
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == input.NewPhone && u.DialCode == input.NewDialCode && u.Id != user.Id);
            if (existingUser != null)
            {
                throw new UserFriendlyException(L(nameof(Exceptions.ObjectIsAlreadyExist), L(nameof(Tokens.PhoneNumber))));
            }

            user.PhoneNumber = input.NewPhone;
            user.DialCode = input.NewDialCode;
            await _userManager.UpdateAsync(user);

            await _registerdPhoneNumberManager.VerifiedPhoneNumberAsync(input.NewDialCode, input.NewPhone);
        }
    }
}

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Abp.UI;
using Abp.Web.Models;
using Abp.Zero.Configuration;
using ReadIraq.Domain.ChangedPhoneNumber;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Accounts.Dto;
using ReadIraq.Authorization.Users;
using ReadIraq.Cities;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Cities.Dto;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.Localization.SourceFiles;
using System;
using System.Threading.Tasks;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Authorization.Accounts
{
    public class AccountAppService : ReadIraqAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex = "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserVerficationCodeManager _userVerficationCodeManager;
        private readonly IRegisterdPhoneNumberManager _registerdPhoneNumberManager;
        private readonly IAttachmentManager _attachmentManager;
        private readonly IRepository<Attachment, long> _attachmentRepository;
        private readonly IRepository<ChangedPhoneNumberForUser> _changedPhoneNumberForUserRepository;
        private readonly IEmailSender _emailSender;
        private readonly ICompanyManager _companyManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly ICompanyBranchManager _companyBranchManager;
        private readonly IRequestForQuotationManager _requestForQuotationManager;
        private readonly IRepository<Grade, int> _gradeRepository;
        private readonly IRepository<City, int> _cityRepository;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager,
            UserManager userManager,
            IPasswordHasher<User> passwordHasher,
            IUserVerficationCodeManager userVerficationCodeManager,
            IAttachmentManager attachmentManager,
            IRegisterdPhoneNumberManager registerdPhoneNumberManager,
            IRepository<Attachment, long> attachmentRepository,
            IRepository<ChangedPhoneNumberForUser> changedPhoneNumberForUserRepository,
            IEmailSender emailSender,
            ICompanyManager companyManager,
            IRepository<User, long> userRepository,
            ICompanyBranchManager companyBranchManager,
            IRequestForQuotationManager requestForQuotationManager,
            IRepository<Grade, int> gradeRepository,
            IRepository<City, int> cityRepository
            )


        {
            _userRegistrationManager = userRegistrationManager;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _userVerficationCodeManager = userVerficationCodeManager;
            _registerdPhoneNumberManager = registerdPhoneNumberManager;
            _attachmentManager = attachmentManager;
            _attachmentRepository = attachmentRepository;
            _changedPhoneNumberForUserRepository = changedPhoneNumberForUserRepository;
            _emailSender = emailSender;
            _companyManager = companyManager;
            _userRepository = userRepository;
            _companyBranchManager = companyBranchManager;
            _requestForQuotationManager = requestForQuotationManager;
            _gradeRepository = gradeRepository;
            _cityRepository = cityRepository;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        /// <summary>
        ///  Is tenant available
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }
        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        [AllowAnonymous]
        [HttpPost]
        [DontWrapResult]
        public async Task<object> RegisterUser(RegisterUserInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                // Validate Phone Existence
                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == input.Phone && u.DialCode == input.DialCode);
                if (existingUser != null)
                {
                    throw new UserFriendlyException(409, L(nameof(Exceptions.ObjectIsAlreadyExist), L(nameof(Tokens.PhoneNumber))));
                }

                // Validate Grade
                if (input.GradeId.HasValue)
                {
                    if (!await _gradeRepository.GetAll().AnyAsync(x => x.Id == input.GradeId.Value))
                    {
                        throw new UserFriendlyException(400, L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.Grade))));
                    }
                }

                // Validate Governorate (City)
                if (input.GovernorateId.HasValue)
                {
                    if (!await _cityRepository.GetAll().AnyAsync(x => x.Id == input.GovernorateId.Value))
                    {
                        throw new UserFriendlyException(400, L(nameof(Exceptions.ObjectWasNotFound), L(nameof(Tokens.City))));
                    }
                }

                var user = await _userRegistrationManager.RegisterUserAsync(
                    input.FullName,
                    input.DialCode,
                    input.Phone,
                    input.Password,
                    input.GradeId,
                    input.GovernorateId,
                    input.UserType
                );

                // Mock OTP logic
                bool otpSent = true;
                string otpCode = "123456";

                return new
                {
                    success = true,
                    data = new
                    {
                        userId = user.Id.ToString(),
                        phone = $"{user.DialCode}{user.PhoneNumber}",
                        otpSent = otpSent,
                        message = $"OTP {otpCode} sent to phone"
                    }
                };
            }
        }

        [AbpAuthorize]
        [HttpPost]
        public async Task AddOrEditUserProfilePhoto(AddUserProfilePhotoDto input)
        {
            var oldAttachment = await _attachmentManager.GetElementByRefAsync(AbpSession.UserId.Value, Enums.Enum.AttachmentRefType.Profile);
            if (oldAttachment is not null)
            {
                await _attachmentManager.DeleteRefIdAsync(oldAttachment);
            }
            Attachment attachment = await _attachmentManager.GetByIdAsync(input.PhotoId);
            await _attachmentManager.UpdateRefIdAsync(attachment, AbpSession.UserId.Value);
        }
        [AbpAuthorize]
        [HttpGet]
        public async Task<ProfileInfoDto> GetProfileInfo()
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                User user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

                var result = ObjectMapper.Map<ProfileInfoDto>(user);
                if (result.EmailAddress.Contains("@EntityFrameWorkCore.net"))
                    result.EmailAddress = null;
                Attachment attachment = new Attachment();
                int companyId = 0;
                int companyBranchId = 0;
                if (user.Type == UserType.CompanyUser)
                {
                    companyId = await _companyManager.GetCompnayIdByUserId(user.Id);
                    attachment = await _attachmentManager.GetElementByRefAsync(companyId, Enums.Enum.AttachmentRefType.CompanyProfile);
                }
                else
                    attachment = await _attachmentManager.GetElementByRefAsync(user.Id, Enums.Enum.AttachmentRefType.Profile);
                if (user.Type == UserType.CompanyBranchUser)
                    companyBranchId = await _companyBranchManager.GetCompnayBranchIdByUserId(user.Id);
                if (attachment is not null)
                {
                    result.ProfilePhoto = new LiteAttachmentDto
                    {

                        Id = attachment.Id,
                        Url = _attachmentManager.GetUrl(attachment),
                        LowResolutionPhotoUrl = _attachmentManager.GetLowResolutionPhotoUrl(attachment),
                    };
                }
                result.CompanyId = companyId;
                result.CompanyBranchId = companyBranchId;
                return result;
            }
        }
        [AbpAuthorize]
        [HttpPut]
        public async Task UpdateProfile(UpdateProfileDto input)
        {
            using (UnitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
                user.Name = input.Name;
                if (user.IsEmailConfirmed == true && input.EmailAddress == user.EmailAddress)
                {
                    user.IsEmailConfirmed = true;
                }
                else
                    user.IsEmailConfirmed = false;
                user.EmailAddress = input.EmailAddress;
                user.RegistrationFullName = input.Name;
                user.LastModificationTime = DateTime.UtcNow;
                Attachment oldAttachments = await _attachmentManager.GetElementByRefAsync((long)user.Id, AttachmentRefType.Profile);
                if (input.ProfilePhoto != 0)
                {
                    if (oldAttachments is not null)
                    {
                        if (input.ProfilePhoto != oldAttachments.Id)
                        {
                            if (oldAttachments is not null)
                                await _attachmentManager.DeleteRefIdAsync(oldAttachments);

                            await _attachmentManager.CheckAndUpdateRefIdAsync(
                                      (long)input.ProfilePhoto, AttachmentRefType.Profile, user.Id);
                        }
                    }
                    else
                    {
                        await _attachmentManager.CheckAndUpdateRefIdAsync(
                                  (long)input.ProfilePhoto, AttachmentRefType.Profile, user.Id);
                    }
                }
                if (input.ProfilePhoto == 0)
                {
                    if (oldAttachments is not null)
                    {
                        await _attachmentRepository.HardDeleteAsync(oldAttachments);
                    }
                }
                await _userManager.UpdateAsync(user);
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }

        }

        /// <summary>
        /// (mobile)Signup with phone number 
        /// this End point just for register new client in system
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<dynamic> SignUpWithPhoneNumberAsync(SignInWithPhoneNumberInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var user = new User();
                if (input.IsFromBasicApp)
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == input.PhoneNumber && x.TenantId == 1);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == input.PhoneNumber && x.TenantId == 2);
                if (user is not null)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.ObjectIsAlreadyExist, Tokens.PhoneNumber));
                }
                else
                {
                    RegisterdPhoneNumber registeredPhoneNumber = await _registerdPhoneNumberManager.AddOrUpdatePhoneNumberAsync(input.DialCode, input.PhoneNumber);
                    //return await _smsSenderAppService.SendOTPVerificationSMS($"{input.DialCode}{input.PhoneNumber}");
                    return await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == input.PhoneNumber && x.TenantId == 2);
                }
            }
        }
        /// <summary>
        ///(mobile) Signin with phone number
        ///this endpoint for login  a registerd client in system this end point return a verification code temporarily
        ///it will be send via sms later
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<dynamic> SignInWithPhoneNumberAsync(SignInWithPhoneNumberInputDto input)
        {
            return await SendVerificationCode(input);
        }
        public async Task LogOut()
        {
            var user = await _userManager.FindByIdAsync(AbpSession.UserId.Value.ToString());
            user.FcmToken = null;
            await _userManager.UpdateAsync(user);
        }
        /// <summary>
        /// Resend verification code
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<dynamic> ResendVerificationCodeAsync(VerifiyPhoneNumberInputDto input)
        {
            if (!input.IsForRegistiration)
                return await SendVerificationCode(input);
            else
            {
                return await SendVerificationCode(input);
                //return await _smsSenderAppService.SendOTPVerificationSMS($"{input.DialCode}{input.PhoneNumber}");
            }

        }

        private async Task<dynamic> SendVerificationCode(SignInWithPhoneNumberInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                string verificationCode = string.Empty;
                var user = new User();
                if (input.IsFromBasicApp)
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == input.PhoneNumber && x.DialCode == input.DialCode && x.TenantId == 1);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == input.PhoneNumber && x.DialCode == input.DialCode && x.TenantId == 2);
                if (user is null)
                {
                    throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PhoneNumber));
                }
                else
                    throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PhoneNumber));
                //return await _smsSenderAppService.SendOTPVerificationSMS($"{input.DialCode}{input.PhoneNumber}");
                //else
                //{
                //    var userVerificationCode = await _userVerficationCodeManager.UpdateVerificationCodeAsync(user.Id, ConfirmationCodeType.ConfirmPhoneNumber);
                //    verificationCode = userVerificationCode.VerficationCode;
                //}
                //return new SignInWithPhoneNumberOutput
                //{
                //    Code = verificationCode
            }   //};
        }
        /// <summary>
        /// Verify SignUp With PhoneNumber
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<dynamic> VerifySignUpWithPhoneNumberAsync(VerifySignUpWithPhoneNumberInputDto input)
        {
            var registerdUser = await _registerdPhoneNumberManager.GetRegisteredPhoneNumberAsync(input.DialCode, input.PhoneNumber);

            if (registerdUser is null)
            {
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.PhoneNumber));
            }
            //if (!await _registerdPhoneNumberManager.CheckVerificationCodeIsValidAsync(input.DialCode, input.PhoneNumber, input.Code))
            //{
            //    throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotValid));
            //}

            //var response = await _smsSenderAppService.VerificationCheckOTP(input.Code, $"{input.DialCode}{input.PhoneNumber}");
            //if (response?.status == "approved" || input.Code == "151997")
            //{
            //
             //   await _registerdPhoneNumberManager.VerifiedPhoneNumberAsync(input.DialCode, input.PhoneNumber);
             //   return response;
            //}
            //else
                throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotCorrect));
        }

        public async Task DeleteAccount()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            if (user.Type != UserType.Admin)
            {

                if (user.Type is (UserType.CompanyBranchUser or UserType.CompanyUser))
                {
                    var hamdi = new EntityDto<int>();
                    hamdi.Id = await _companyManager.GetCompnayIdByUserId(user.Id);
                    return;
                }
                await _userManager.DeleteAsync(user);
                if (user.Type is not (UserType.CompanyBranchUser or UserType.CompanyUser))
                    await _requestForQuotationManager.DeleteAllRequestForUserIfFound(user.Id);
            }
            else
                throw new UserFriendlyException(403, Exceptions.YouCannotDoThisAction);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize]
        [HttpPut]
        public async Task<SignInWithPhoneNumberOutput> ChangePhoneNumber(ChangePhoneNumberDto input)
        {
            ChangedPhoneNumberForUser changedPhone = new ChangedPhoneNumberForUser();
            changedPhone.NewPhoneNumber = input.PhoneNumber;
            changedPhone.NewDialCode = input.DialCode;
            changedPhone.UserId = AbpSession.UserId.Value;
            //var userVerificationCode = await _userVerficationCodeManager.UpdateVerificationCodeAsync(AbpSession.UserId.Value, ConfirmationCodeType.ConfirmPhoneNumber);
            //await _smsSenderAppService.SendOTPVerificationSMS($"{input.DialCode}{input.PhoneNumber}");
            await _changedPhoneNumberForUserRepository.InsertAsync(changedPhone);

            return new SignInWithPhoneNumberOutput();
            //{
            //    //Code = userVerificationCode.VerficationCode
            //};
        }
        /// <summary>
        /// Call This To Send Verification Code To Email For Confirm Email Or ForgetPassword
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [AllowAnonymous]
        public async Task<SendCodeToEmailOutPut> SendVerificationCodeToEmailAsync(RequestCodeToEmailInput input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                Random generator = new Random();
                string verificationCode = string.Empty;
                var user = new User();
                if (input.IsFromBasicApp)
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.EmailAddress == input.EmailAddress && x.TenantId == 1);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(x => x.EmailAddress == input.EmailAddress && x.TenantId == 2);
                if (user is null)
                    throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.EmailAddress));

                var userVerificationCode = await _userVerficationCodeManager.AddOrUpdateUserVerficationCodeForEmailAsync(
                                  new UserVerficationCode
                                  {
                                      ConfirmationCodeType = ConfirmationCodeType.ConfirmEmail,
                                      UserId = user.Id,
                                      VerficationCode = generator.Next(0, 1000000).ToString("D6"),
                                      IsForEmail = true
                                  });
                //await _emailSenderAppService.SendEmail(input.EmailAddress, userVerificationCode.VerficationCode, input.CodeType);

                return new SendCodeToEmailOutPut
                {
                    Code = userVerificationCode.VerficationCode
                };
            }
        }
        /// <summary>
        /// Verify Email Account
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<Boolean> VerifyUserEmailAccount([FromBody] ConfirmEmailWithCodeInputDto input)
        {
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var registerdUser = new User();
                if (input.IsFromBasicApp)
                    registerdUser = await _userManager.Users.FirstOrDefaultAsync(x => x.EmailAddress == input.EmailAddress && x.TenantId == 1);
                else
                    registerdUser = await _userManager.Users.FirstOrDefaultAsync(x => x.EmailAddress == input.EmailAddress && x.TenantId == 2);
                if (registerdUser is not null)
                {
                    var userCode = await _userVerficationCodeManager.GetUserWithVerificationCodeAsync(registerdUser.Id, ConfirmationCodeType.ConfirmEmail, true);
                    if (!await _userVerficationCodeManager.CheckVerificationCodeIsValidAsync(registerdUser.Id, ConfirmationCodeType.ConfirmEmail, true))
                    {
                        throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotValid));
                    }
                    if (userCode.VerficationCode.Equals(input.Code))
                    {
                        registerdUser.IsEmailConfirmed = true;
                        var user = await _userRepository.UpdateAsync(registerdUser);
                        await UnitOfWorkManager.Current.SaveChangesAsync();
                        return true;
                    }
                    throw new UserFriendlyException(string.Format(Exceptions.VerificationCodeIsnotCorrect));
                }
                throw new UserFriendlyException(string.Format(Exceptions.ObjectWasNotFound, Tokens.User));
            }
        }
        [HttpGet]
        [AbpAuthorize]
        public async Task<ActivationStatuesDto> CheckIfUserIsActiveOrNot()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            return new ActivationStatuesDto { IsActive = user.IsActive };
        }
    }
}

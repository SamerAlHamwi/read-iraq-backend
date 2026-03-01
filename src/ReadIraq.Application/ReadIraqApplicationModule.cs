using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using AutoMapper;
using ReadIraq.Authorization;
using ReadIraq.Authorization.Accounts.Dto;
using ReadIraq.Authorization.Users;
using ReadIraq.Cities.Dto;
using ReadIraq.ContactUsService.Dto;
using ReadIraq.Countries;
using ReadIraq.Countries.Dto;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.ContactUses;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.FrequentlyQuestions.Dto;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.Regions.Dto;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.Terms;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Translations.Dto;
using ReadIraq.Grades.Dto;
using ReadIraq.PrivacyPolicyService.Dto;
using ReadIraq.PushNotifications.Dto;
using ReadIraq.Subjects.Dto;
using ReadIraq.Teachers.Dto;
using ReadIraq.TermService.Dto;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Enrollments.Dto;
using ReadIraq.Domain.UserSessionProgresses;
using ReadIraq.UserSessionProgresses.Dto;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Subscriptions.Dto;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Quizzes.Dto;
using ReadIraq.Domain.Notifications;
using ReadIraq.Notifications.Dto;
using ReadIraq.Domain.Audit;
using ReadIraq.Audit.Dto;
using ReadIraq.Domain.Settings;
using ReadIraq.Settings.Dto;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.LessonSessions.Dto;
using ReadIraq.NotificationService;
using ReadIraq.Users.Dto;
using ReadIraq.LessonSessions.Dto;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.Comments;
using ReadIraq.Comments.Dto;
using System.Linq;

namespace ReadIraq
{
    [DependsOn(
        typeof(ReadIraqCoreModule),
        typeof(AbpAutoMapperModule))]
    public class ReadIraqApplicationModule : AbpModule
    {
        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<NotificationBackgroundWorker>());
        }
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ReadIraqAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ReadIraqApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            ); Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                CustomDtoMapper.CreateMappings(configuration, new MultiLingualMapContext(
                    IocManager.Resolve<ISettingManager>()
                ));
            });
        }
        public static class CustomDtoMapper
        {
            public static void CreateMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
            {

                #region User
                configuration.CreateMap<User, UserDetailDto>();
                configuration.CreateMap<User, ProfileInfoDto>();
                configuration.CreateMap<User, UserDto>();
                #endregion

                #region Grade
                configuration.CreateMap<Grade, GradeDto>();
                configuration.CreateMap<Grade, LiteGradeDto>();
                #endregion

                #region TeacherFeature
                configuration.CreateMap<TeacherFeature, TeacherFeatureDto>();
                configuration.CreateMap<TeacherFeature, LiteTeacherFeatureDto>();
                #endregion

                #region TeacherProfile
                configuration.CreateMap<TeacherProfile, TeacherProfileDto>()
                    .ForMember(dest => dest.Features, opt => opt.Ignore())
                    .ForMember(dest => dest.Subjects, opt => opt.Ignore());
                configuration.CreateMap<TeacherProfile, LiteTeacherProfileDto>();
                configuration.CreateMap<TeacherRatingBreakdown, TeacherRatingBreakdownDto>();
                #endregion

                #region TeacherReview
                configuration.CreateMap<TeacherReview, TeacherReviewDto>();
                #endregion

                #region Subject
                configuration.CreateMap<Subject, SubjectDto>();
                configuration.CreateMap<Subject, LiteSubjectDto>();
                #endregion

                #region LessonSession
                configuration.CreateMap<LessonSession, LessonSessionDto>();
                configuration.CreateMap<LessonSession, LiteLessonSessionDto>();
                configuration.CreateMap<LessonSessionAttachment, LiteAttachmentDto>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AttachmentId))
                    .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Attachment.Url))
                    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Attachment.FileName))
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Attachment.Type));
                #endregion

                #region Translation
                configuration.CreateMap<Translation, TranslationDto>().ReverseMap();
                #endregion

                #region PushNotification
                configuration.CreateMultiLingualMap<PushNotification, PushNotificationTranslation, PushNotificationDetailsDto>(context).TranslationMap
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
                configuration.CreateMultiLingualMap<PushNotification, PushNotificationTranslation, PushNotificationDto>(context).TranslationMap
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
                #endregion


                #region Country
                // Country Translation Configuration
                configuration.CreateMultiLingualMap<Country, CountryTranslation, CountryDetailsDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<Country, CountryTranslation, CountryDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                #endregion

                #region City
                // City Translation Configuration
                configuration.CreateMultiLingualMap<City, CityTranslation, LiteCityDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<City, CityTranslation, CityDetailsDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<City, CityTranslation, CityDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<City, CityTranslation, LiteCity>(context).TranslationMap
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                #endregion

                #region Region
                // Region Translation Configuration
                configuration.CreateMultiLingualMap<Region, RegionTranslation, LiteRegionDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<Region, RegionTranslation, LiteRegionCityDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<Region, RegionTranslation, RegionDetailsDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                configuration.CreateMultiLingualMap<Region, RegionTranslation, RegionDto>(context).TranslationMap
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
                #endregion

                #region ContactUs
                configuration.CreateMultiLingualMap<ContactUs, ContactUsTranslation, ContactUsDetailsDto>(context).TranslationMap
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
                #endregion

                #region PrivacyPolicy
                configuration.CreateMultiLingualMap<PrivacyPolicy, PrivacyPolicyTranslation, PrivacyPolicyDetailsDto>(context).TranslationMap
                 .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
                configuration.CreateMultiLingualMap<PrivacyPolicy, PrivacyPolicyTranslation, LitePrivacyPolicyDto>(context).TranslationMap
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
                #endregion

                #region Term
                configuration.CreateMultiLingualMap<Term, TermTranslation, TermDetailsDto>(context).TranslationMap
                 .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
                configuration.CreateMultiLingualMap<Term, TermTranslation, LiteTermDto>(context).TranslationMap
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
                #endregion

                #region FrequentlyQuestion
                configuration.CreateMultiLingualMap<FrequentlyQuestion, FrequentlyQuestionTranslation, FrequentlyQuestionDetailsDto>(context).TranslationMap
                 .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                 .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer));
                configuration.CreateMultiLingualMap<FrequentlyQuestion, FrequentlyQuestionTranslation, LiteFrequentlyQuestionDto>(context).TranslationMap
                .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question))
                 .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer));
                #endregion

                #region Enrollment
                configuration.CreateMap<Enrollment, EnrollmentDto>();
                configuration.CreateMap<CreateEnrollmentDto, Enrollment>();
                configuration.CreateMap<UpdateEnrollmentDto, Enrollment>();
                #endregion

                #region UserSessionProgress
                configuration.CreateMap<UserSessionProgress, UserSessionProgressDto>();
                configuration.CreateMap<CreateUserSessionProgressDto, UserSessionProgress>();
                configuration.CreateMap<UpdateUserSessionProgressDto, UserSessionProgress>();
                #endregion

                #region LessonReport
                configuration.CreateMap<LessonReport, LessonReportDto>();
                configuration.CreateMap<CreateLessonReportDto, LessonReport>();
                #endregion

                #region TeacherReport
                configuration.CreateMap<TeacherReport, TeacherReportDto>();
                configuration.CreateMap<CreateTeacherReportDto, TeacherReport>();
                #endregion

                #region Subscription
                configuration.CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
                configuration.CreateMap<CreateSubscriptionPlanDto, SubscriptionPlan>();
                configuration.CreateMap<UpdateSubscriptionPlanDto, SubscriptionPlan>();
                configuration.CreateMap<SubscriptionFeature, SubscriptionFeatureDto>();
                configuration.CreateMap<SubscriptionFeatureMap, SubscriptionFeatureMapDto>();
                configuration.CreateMap<Subscription, SubscriptionDto>();
                configuration.CreateMap<CreateSubscriptionDto, Subscription>();
                configuration.CreateMap<UpdateSubscriptionDto, Subscription>();
                #endregion

                #region Quizzes
                configuration.CreateMap<Quiz, QuizDto>();
                configuration.CreateMap<CreateQuizDto, Quiz>();
                configuration.CreateMap<Question, QuestionDto>();
                configuration.CreateMap<CreateQuestionDto, Question>();
                configuration.CreateMap<QuizAttempt, QuizAttemptDto>();
                configuration.CreateMap<CreateQuizAttemptDto, QuizAttempt>();
                #endregion

                #region Notifications
                configuration.CreateMap<AppNotification, AppNotificationDto>();
                configuration.CreateMap<CreateAppNotificationDto, AppNotification>();
                #endregion

                #region Audit
                configuration.CreateMap<ActivityLog, ActivityLogDto>();
                configuration.CreateMap<CreateActivityLogDto, ActivityLog>();
                #endregion

                #region Settings
                configuration.CreateMap<AppSetting, AppSettingDto>();
                configuration.CreateMap<CreateAppSettingDto, AppSetting>();
                #endregion

                #region SessionComment
                configuration.CreateMap<SessionComment, SessionCommentDto>();
                configuration.CreateMap<CreateSessionCommentDto, SessionComment>();
                configuration.CreateMap<UpdateSessionCommentDto, SessionComment>();
                #endregion
            }
        }

    }
}

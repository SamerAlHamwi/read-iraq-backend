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
                configuration.CreateMap<TeacherProfile, TeacherProfileDto>();
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


            }
        }

    }
}

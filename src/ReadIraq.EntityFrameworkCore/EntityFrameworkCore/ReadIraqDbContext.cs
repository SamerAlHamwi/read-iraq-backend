using Abp.Zero.EntityFrameworkCore;
using ReadIraq.Domain.ChangedPhoneNumber;
using Microsoft.EntityFrameworkCore;
using ReadIraq.Authorization.Roles;
using ReadIraq.Authorization.Users;
using ReadIraq.Countries;
using ReadIraq.Domain.Advertisiments;
using ReadIraq.Domain.ApkBuilds;
using ReadIraq.Domain.AskForHelps;
using ReadIraq.Domain.Attachments;
using ReadIraq.Domain.AttributeAndAttachments;
using ReadIraq.Domain.AttributeAndAttachmentsForDrafts;
using ReadIraq.Domain.AttributeChoices;
using ReadIraq.Domain.AttributeForSourceTypeValuesForDrafts;
using ReadIraq.Domain.AttributeForSourcTypeValues;
using ReadIraq.Domain.AttributesForSourceType;
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.Codes;
using ReadIraq.Domain.CommissionGroups;
using ReadIraq.Domain.Companies;
using ReadIraq.Domain.CompanyBranches;
using ReadIraq.Domain.ContactUses;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.Drafts;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.Mediators;
using ReadIraq.Domain.MoneyTransfers;
using ReadIraq.Domain.Offers;
using ReadIraq.Domain.PaidRequestPossibles;
using ReadIraq.Domain.Partners;
using ReadIraq.Domain.Points;
using ReadIraq.Domain.PointsValues;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domain.RejectReasons;
using ReadIraq.Domain.RequestForQuotationContacts;
using ReadIraq.Domain.RequestForQuotationContactsForDrafts;
using ReadIraq.Domain.RequestForQuotations;
using ReadIraq.Domain.Reviews;
using ReadIraq.Domain.SearchedPlacesByUsers;
using ReadIraq.Domain.SelectedCompaniesByUsers;
using ReadIraq.Domain.services;
using ReadIraq.Domain.ServiceValueForOffers;
using ReadIraq.Domain.ServiceValues;
using ReadIraq.Domain.ServiceValuesForDrafts;
using ReadIraq.Domain.SourceTypes;
using ReadIraq.Domain.SubServices;
using ReadIraq.Domain.Terms;
using ReadIraq.Domain.TimeWorks;
using ReadIraq.Domain.Toolss;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.MultiTenancy;

namespace ReadIraq.EntityFrameworkCore
{
    public class ReadIraqDbContext : AbpZeroDbContext<Tenant, Role, User, ReadIraqDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryTranslation> CountryTranslations { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CityTranslation> CityTranslations { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<RegionTranslation> RegionTranslations { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<PushNotification> PushNotifications { get; set; }
        public virtual DbSet<UserVerficationCode> UserVerficationCodes { get; set; }
        public virtual DbSet<RegisterdPhoneNumber> RegisterdPhoneNumbers { get; set; }
        public virtual DbSet<RequestForQuotation> RequestForQuotations { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyTranslation> CompanyTranslations { get; set; }
        public virtual DbSet<AttributeForSourceType> AttributeForSourcTypes { get; set; }
        public virtual DbSet<AttributeForSourceTypeValue> AttributeForSourceTypeValues { get; set; }
        public virtual DbSet<RequestForQuotationContact> RequestForQuotationContacts { get; set; }
        public virtual DbSet<Advertisiment> Advertisiments { get; set; }
        public virtual DbSet<AdvertisimentPosition> AdvertisimentPositions { get; set; }
        public virtual DbSet<AttributeForSourceTypeTranslation> AttributeForSourceTypeTranslations { get; set; }
        public virtual DbSet<SourceType> SourceTypes { get; set; }
        public virtual DbSet<SourceTypeTranslation> SourceTypeTranslations { get; set; }
        public virtual DbSet<AttributeChoice> AttributeChoices { get; set; }
        public virtual DbSet<AttributeChoiceTranslation> AttributeChoiceTranslations { get; set; }
        public virtual DbSet<ServiceTranslation> ServiceTranslations { get; set; }
        public virtual DbSet<SubService> SubServices { get; set; }
        public virtual DbSet<SubServiceTranslation> SubServiceTranslations { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public virtual DbSet<PrivacyPolicyTranslation> PrivacyPolicyTranslations { get; set; }
        public virtual DbSet<ServiceValue> ServiceValues { get; set; }
        public virtual DbSet<AttributeChoiceAndAttachment> AttributeChoiceAndAttachments { get; set; }
        public virtual DbSet<CompanyBranch> CompanyBranches { get; set; }
        public virtual DbSet<CompanyContact> CompanyContacts { get; set; }
        public virtual DbSet<Tool> Tools { get; set; }
        public virtual DbSet<ToolTranslation> ToolTranslations { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<TermTranslation> TermTranslations { get; set; }
        public virtual DbSet<Mediator> Mediator { get; set; }
        public virtual DbSet<Partner> Partner { get; set; }
        public virtual DbSet<SelectedCompaniesBySystemForRequest> SelectedCompaniesBySystemForRequests { get; set; }
        public virtual DbSet<Offer> Offers { get; set; }
        public virtual DbSet<ServiceValueForOffer> ServiceValueForOffers { get; set; }
        public virtual DbSet<CompanyBranchTranslation> CompanyBranchTranslation { get; set; }
        public virtual DbSet<AskForHelp> AskForHelps { get; set; }
        public virtual DbSet<FrequentlyQuestion> FrequentlyQuestions { get; set; }
        public virtual DbSet<FrequentlyQuestionTranslation> FrequentlyQuestionTranslations { get; set; }
        public virtual DbSet<RejectReason> RejectReasons { get; set; }
        public virtual DbSet<RejectReasonTranslation> RejectReasonTranslations { get; set; }
        public virtual DbSet<Draft> Drafts { get; set; }
        public virtual DbSet<AttributeForSourceTypeValuesForDraft> AttributeForSourceTypeValuesForDrafts { get; set; }
        public virtual DbSet<ServiceValuesForDraft> ServiceValuesForDrafts { get; set; }
        public virtual DbSet<RequestForQuotationContactsForDraft> RequestForQuotationContactsForDrafts { get; set; }
        public virtual DbSet<AttributeAndAttachmentsForDraft> AttributeAndAttachmentsForDrafts { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ChangedPhoneNumberForUser> ChangedPhoneNumberForUsers { get; set; }
        public virtual DbSet<Code> Codes { get; set; }
        public virtual DbSet<Point> Points { get; set; }
        public virtual DbSet<PointTranslation> PointTranslations { get; set; }
        public virtual DbSet<PaidRequestPossible> PaidRequestPossibles { get; set; }
        public virtual DbSet<PointsValue> PointsValue { get; set; }
        public virtual DbSet<SearchedPlacesByUser> SearchedPlacesByUsers { get; set; }
        public virtual DbSet<CommissionGroup> CommissionGroups { get; set; }
        public virtual DbSet<TimeWork> TimeWork { get; set; }
        public virtual DbSet<ApkBuild> ApkBuilds { get; set; }
        public virtual DbSet<MoneyTransfer> MoneyTransfers { get; set; }

        public ReadIraqDbContext(DbContextOptions<ReadIraqDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // 2. Decimal Property Issues
            modelBuilder.Entity<Code>()
                .Property(c => c.DiscountPercentage)
                .HasColumnType("decimal(18,2)");
            // Other configurations...

            base.OnModelCreating(modelBuilder);
        }
    }
}

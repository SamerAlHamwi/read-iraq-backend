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
using ReadIraq.Domain.Cities;
using ReadIraq.Domain.ContactUses;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.Mediators;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domain.Reviews;
using ReadIraq.Domain.Terms;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.MultiTenancy;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using System;

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
        public virtual DbSet<Advertisiment> Advertisiments { get; set; }
        public virtual DbSet<AdvertisimentPosition> AdvertisimentPositions { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }
        public virtual DbSet<PrivacyPolicyTranslation> PrivacyPolicyTranslations { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<TermTranslation> TermTranslations { get; set; }
        public virtual DbSet<Mediator> Mediator { get; set; }
        public virtual DbSet<AskForHelp> AskForHelps { get; set; }
        public virtual DbSet<FrequentlyQuestion> FrequentlyQuestions { get; set; }
        public virtual DbSet<FrequentlyQuestionTranslation> FrequentlyQuestionTranslations { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<ChangedPhoneNumberForUser> ChangedPhoneNumberForUsers { get; set; }
        public virtual DbSet<ApkBuild> ApkBuilds { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<GradeGroup> GradeGroups { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<GradeSubject> GradeSubjects { get; set; }

        public virtual DbSet<TeacherFeature> TeacherFeatures { get; set; }
        public virtual DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public virtual DbSet<TeacherFeatureMap> TeacherFeaturesMap { get; set; }
        public virtual DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public virtual DbSet<TeacherRatingBreakdown> TeacherRatingBreakdowns { get; set; }
        public virtual DbSet<TeacherReview> TeacherReviews { get; set; }

        public ReadIraqDbContext(DbContextOptions<ReadIraqDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GradeSubject>(b =>
            {
                b.HasIndex(e => new { e.GradeId, e.SubjectId }).IsUnique();
            });

            modelBuilder.Entity<Translation>(b =>
            {
                b.Property<Guid?>("GradeGroupId");
                b.Property<int?>("GradeId");
                b.Property<Guid?>("SubjectId");
            });

            modelBuilder.Entity<Grade>(b =>
            {
                b.HasMany(e => e.Name).WithOne().HasForeignKey("GradeId");
            });

            modelBuilder.Entity<Subject>(b =>
            {
                b.HasMany(e => e.Name).WithOne().HasForeignKey("SubjectId");
            });

            modelBuilder.Entity<GradeGroup>(b =>
            {
                b.HasMany(e => e.Name).WithOne().HasForeignKey("GradeGroupId");
            });

            modelBuilder.Entity<TeacherFeatureMap>(b =>
            {
                b.HasIndex(e => new { e.TeacherProfileId, e.TeacherFeatureId }).IsUnique();
            });

            modelBuilder.Entity<TeacherSubject>(b =>
            {
                b.HasIndex(e => new { e.TeacherProfileId, e.SubjectId }).IsUnique();
            });

            modelBuilder.Entity<TeacherRatingBreakdown>(b =>
            {
                b.HasIndex(e => new { e.TeacherProfileId, e.Rating }).IsUnique();
            });
        }
    }
}

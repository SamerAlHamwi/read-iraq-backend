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
using ReadIraq.Domain.Comments;
using ReadIraq.Domain.ContactUses;
using ReadIraq.Domain.Countries;
using ReadIraq.Domain.Follows;
using ReadIraq.Domain.FrequentlyQuestions;
using ReadIraq.Domain.LessonSessions;
using ReadIraq.Domain.Mediators;
using ReadIraq.Domain.PrivacyPolicies;
using ReadIraq.Domain.PushNotifications;
using ReadIraq.Domain.Regions;
using ReadIraq.Domain.RegisterdPhoneNumbers;
using ReadIraq.Domain.Reviews;
using ReadIraq.Domain.SavedItems;
using ReadIraq.Domain.Terms;
using ReadIraq.Domains.UserVerficationCodes;
using ReadIraq.MultiTenancy;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Translations;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Teachers;
using ReadIraq.Domain.Enrollments;
using ReadIraq.Domain.UserSessionProgresses;
using ReadIraq.Domain.Subscriptions;
using ReadIraq.Domain.Quizzes;
using ReadIraq.Domain.Notifications;
using ReadIraq.Domain.Audit;
using ReadIraq.Domain.Settings;
using ReadIraq.Domain.Gifts;
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
        public virtual DbSet<LessonSession> LessonSessions { get; set; }
        public virtual DbSet<LessonSessionAttachment> LessonSessionAttachments { get; set; }
        public virtual DbSet<SessionComment> SessionComments { get; set; }
        public virtual DbSet<LessonReport> LessonReports { get; set; }
        public virtual DbSet<UserFollowTeacher> UserFollowTeachers { get; set; }
        public virtual DbSet<UserSavedItem> UserSavedItems { get; set; }
        public virtual DbSet<PushNotification> PushNotifications { get; set; }
        public virtual DbSet<UserVerficationCode> UserVerficationCodes { get; set; }
        public virtual DbSet<RegisterdPhoneNumber> RegisterdPhoneNumbers { get; set; }
        public virtual DbSet<Advertisiment> Advertisiments { get; set; }
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
        public virtual DbSet<TeacherReport> TeacherReports { get; set; }
        public virtual DbSet<UserPreferredTeacher> UserPreferredTeachers { get; set; }

        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<UserSessionProgress> UserSessionProgresses { get; set; }
        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<SubscriptionFeature> SubscriptionFeatures { get; set; }
        public virtual DbSet<SubscriptionFeatureMap> SubscriptionFeaturesMap { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<Gift> Gifts { get; set; }

        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }
        public virtual DbSet<AppNotification> AppNotifications { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }

        public ReadIraqDbContext(DbContextOptions<ReadIraqDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LessonSessionAttachment>(b =>
            {
                b.HasKey(x => new { x.LessonSessionId, x.AttachmentId });
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<SessionComment>(b =>
            {
                b.HasOne(x => x.ParentComment)
                    .WithMany(x => x.Replies)
                    .HasForeignKey(x => x.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<SessionComment>().HasOne(x => x.LessonSession)
                    .WithMany()
                    .HasForeignKey(x => x.LessonSessionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<LessonReport>(b =>
            {
                b.HasOne(x => x.LessonSession)
                    .WithMany()
                    .HasForeignKey(x => x.LessonSessionId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserFollowTeacher>(b =>
            {
                b.HasIndex(x => new { x.UserId, x.TeacherProfileId }).IsUnique();

                b.HasOne(x => x.TeacherProfile)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherProfileId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserSavedItem>(b =>
            {
                b.HasIndex(x => new { x.UserId, x.ItemType, x.ItemId }).IsUnique();
            });

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

            modelBuilder.Entity<TeacherReview>(b =>
            {
                b.HasOne(x => x.TeacherProfile)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TeacherReport>(b =>
            {
                b.HasOne(x => x.TeacherProfile)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherProfileId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<UserPreferredTeacher>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.SubjectId, e.TeacherProfileId }).IsUnique();

                b.HasOne(x => x.TeacherProfile)
                    .WithMany()
                    .HasForeignKey(x => x.TeacherProfileId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Subject)
                    .WithMany()
                    .HasForeignKey(x => x.SubjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Enrollment>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.SubjectId }).IsUnique();
                b.Property(e => e.ProgressPercent).HasPrecision(18, 2);
            });

            modelBuilder.Entity<UserSessionProgress>(b =>
            {
                b.HasIndex(e => new { e.UserId, e.SessionId }).IsUnique();

                b.HasOne(x => x.Session)
                    .WithMany()
                    .HasForeignKey(x => x.SessionId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<SubscriptionFeatureMap>(b =>
            {
                b.HasIndex(e => new { e.PlanId, e.FeatureId }).IsUnique();
            });

            modelBuilder.Entity<SubscriptionPlan>(b =>
            {
                b.Property(e => e.Price).HasPrecision(18, 2);
                b.Property(e => e.PriceBeforeDiscount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ActivityLog>(b =>
            {
                b.HasOne(x => x.Actor)
                    .WithMany()
                    .HasForeignKey(x => x.ActorId)
                    .IsRequired(false);
            });

            modelBuilder.Entity<AppSetting>(b =>
            {
                b.HasIndex(e => e.Key).IsUnique();
            });

            modelBuilder.Entity<Gift>(b =>
            {
                b.HasOne(x => x.TargetUser)
                    .WithMany()
                    .HasForeignKey(x => x.TargetUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.AdminUser)
                    .WithMany()
                    .HasForeignKey(x => x.AdminUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}

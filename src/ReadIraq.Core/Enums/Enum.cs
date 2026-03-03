using ReadIraq.Localization.SourceFiles;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Enums
{
    public class Enum
    {
        public enum SubjectLevel : byte
        {
            FirstTerm = 1,
            SecondTerm = 2,
            Semester = 3
        }

        public enum ConfirmationCodeType : byte
        {
            [Display(ResourceType = typeof(Tokens), Name = nameof(Tokens.ForgetPassword))]
            ForgotPassword = 1,
            [Display(ResourceType = typeof(Tokens), Name = nameof(Tokens.ConfirmPhoneNumber))]
            ConfirmPhoneNumber = 2,
            [Display(ResourceType = typeof(Tokens), Name = nameof(Tokens.ConfirmEmail))]
            ConfirmEmail = 3,

        }
        public enum AttachmentRefType : byte
        {
            Profile = 1,
            Advertisiment = 2,
            Subject = 3,
            LessonSessionThumbnail = 4,
            LessonSessionVideo = 5,
            TeacherProfile = 6,
            LessonSessionOther = 7,
            Other = 8,
            Question = 9
        }
        public enum AttachmentType : byte
        {
            PDF = 1,
            WORD = 2,
            JPEG = 3,
            PNG = 4,
            JPG = 5,
            MP4 = 6,
            MP3 = 7,
            APK = 8,
        }
        /// <summary>
        /// Generic media type classification for attachments.
        /// </summary>
        public enum MediaType : byte
        {
            Video = 1,
            Pdf = 2,
            Image = 3,
            Audio = 4,
            Other = 5
        }

        public enum SavedItemType : byte
        {
            Session = 1,
            Subject = 2,
            Teacher = 3,
            Other = 4
        }
        public enum ImageType : byte
        {
            JPEG = 1,
            PNG = 2,
            JPG = 3
        }

        public enum UserType : byte
        {
            [Display(ResourceType = typeof(Tokens), Name = nameof(Tokens.Admin))]
            SuperAdmin = 1,
            Student = 2,
            Teacher = 3
        }

        public enum PositionForAdvertisiment : byte
        {
            Top = 1,
            InBetween = 2
        }

        public enum AskForHelpStatues : byte
        {
            Waiting = 1,
            Followed = 2
        }

        public enum CodeType : byte
        {
            DiscountPercentageValue = 1,
            FixedValue = 2
        }

        public enum SystemType : byte
        {
            Android = 1,
            Ios = 2,
        }

        public enum UpdateOptions : byte
        {
            Optional = 1,
            Mandatory = 2,
            Nothing = 3
        }

        public enum QuestionType : byte
        {
            MCQ = 1,
            TrueFalse = 2,
            ShortAnswer = 3
        }

        public enum NotificationType : byte
        {
            DAILY_STUDY_REMINDER = 1,
            NEW_LESSON_UPLOADED = 2,
            QUIZ_REMINDER_TO_TAKE = 3,
            QUIZ_PASSED_HIGH_SCORE = 4,
            WEEKLY_PROGRESS_REPORT = 5,
            TEACHER_REPLY = 6,
            STREAK_BROKEN = 7,
            SYSTEM_MAINTENANCE = 8,
            CONTENT_RECOMMENDATION = 9
        }

        public enum NotificationPriority : byte
        {
            LOW = 1,
            DEFAULT = 2,
            HIGH = 3
        }

        public enum NotificationChannel : byte
        {
            PUSH = 1,
            IN_APP = 2,
            EMAIL = 3,
            SMS = 4
        }

        public enum NotificationDeliveryStatus : byte
        {
            PENDING = 1,
            SENT = 2,
            FAILED = 3
        }
    }
}

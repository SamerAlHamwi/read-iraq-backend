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
            RequestForQuotation = 2,
            Advertisiment = 3,
            QR = 4,
            SourceTypeIcon = 5,
            ContactUs = 6,
            Service = 7,
            CompanyProfile = 8,
            CompanyOwnerIdentity = 9,
            CompanyCommercialRegister = 10,
            AdditionalAttachment = 11,
            SubService = 12,
            Tool = 13,
            Draft = 14,
            FinishedRequestByCompany = 15

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
            Admin = 1,
            BasicUser = 2,
            CompanyUser = 3,
            CustomerService = 4,
            CompanyBranchUser = 5,
            MediatorUser = 6,
            Student = 7,
            Teacher = 8,
            SuperAdmin = 9
        }


        public enum TopicType : byte
        {
            All = 0,
            Admin = 1,
            BasicUser = 2,
            CompanyUser = 3,
            CompanyBranchUser = 4,
            BrokerUser = 5,
            CustomerUser = 6
        }

        public enum ServiceType : byte
        {
            Internal = 1,
            External = 2,
            Both = 3
        }

        public enum RequestForQuotationContactType : byte
        {
            Source = 1,
            Destination = 2,
        }
        public enum PositionForAdvertisiment : byte
        {
            Top = 1,
            InBetween = 2
        }
        public enum Screen : byte
        {
            Home = 1
        }
        //public enum ToolRelationType : byte
        //{
        //    ToolIdForService = 1,
        //    ToolIdForSubService = 2,
        //}
        public enum ServiceValueType : byte
        {
            ForUser = 1,
            ForCompany = 2,
            ForCompanyBranch = 3,
        }
        public enum CompanyStatues : byte
        {
            Checking = 1,
            Approved = 2,
            Rejected = 3,
            RejectedNeedToEdit = 4
        }
        public enum CompanyBranchStatues : byte
        {
            Checking = 1,
            Approved = 2,
            Rejected = 3,
            RejectedNeedToEdit = 4
        }
        public enum RequestForQuotationStatues : byte
        {
            Checking = 1,
            Approved = 2,
            Rejected = 3,
            Possible = 4,
            HasOffers = 5,
            InProcess = 6,
            FinishByCompany = 7,
            FinishByUser = 8,
            NotFinishByUser = 9,
            Finished = 10,
            Canceled = 11,
            CanceledAfterRejectOffers = 12,
            OutOfPossible = 13,
            CanceledAfterInProcess = 14,
            RejectedNeedToEdit = 15,
        }
        public enum OfferStatues : byte
        {
            Checking = 1,
            Approved = 2,
            Rejected = 3,
            SelectedByUser = 4,
            RejectedByUser = 5,
            Finished = 6,
            RejectedNeedToEdit = 7
        }
        public enum OfferProvider : byte
        {
            Company = 1,
            BranchCompany = 2
        }
        public enum AskForHelpStatues : byte
        {
            Waiting = 1,
            Followed = 2
        }


        public enum RatingType : byte
        {

            ForCompany = 1,
            ForCompanyBranch = 2,
        }

        //public enum AskEditStatus : byte
        //{
        //    NoModificationRequest = 1,
        //    ModificationRequest = 2,
        //    ModificationRequestAccept = 3,
        //    ModificationRequestReject = 4,


        //}

        public enum CodeType : byte
        {
            DiscountPercentageValue = 1,
            FixedValue = 2
        }
        public enum PossibilityPotentialClient : byte
        {
            PotentialClient = 1,
            NotPotentialClient = 2

        }
        public enum ReviewProvideType : byte
        {

            ForCompany = 1,
            ForCompanyBranch = 2,
        }
        public enum Provider : byte
        {
            Company = 1,
            CompanyBranch = 2,
        }
        public enum SystemType : byte
        {
            Android = 1,
            Ios = 2,
        }
        public enum AppType : byte
        {
            Basic = 1,
            Partner = 2,
            Both = 3
        }
        public enum UpdateOptions : byte
        {
            Optional = 1,
            Mandatory = 2,
            Nothing = 3
        }
        public enum ReasonOfPaid : byte
        {
            PayForOffer = 1,
            BuyBundle = 2,
            BuyFeatureBundle = 3,
            ReturnMoneyAfterDiscount = 4,
            ReturnMoneyWithoutDiscount = 5,
            PayForExtendStorage = 6,
        }
        public enum PaidStatues : byte
        {
            Finish = 1,

        }
        public enum PaidProvider : byte
        {
            User = 1,
            CompanyUser = 2,
            CompanyBranchUser = 3,
        }
        public enum PaidDestination : byte
        {
            ForHim = 1,
            OnHim = 2
        }
    }
}

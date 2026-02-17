using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Microsoft.AspNetCore.Http;
using ReadIraq.Domain.Attachments;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Attachments.Dto
{
    /// <summary>
    /// UploadAttachmentInputDto
    /// </summary>
    [AutoMapTo(typeof(Attachment))]
    public class UploadAttachmentInputDto : ICustomValidate
    {
        /// <summary> 
        ///  Profile = 1,
        ///RequestForQuotation = 2,
        ///Advertisiment = 3,
        ///QR = 4,
        ///SourceTypeIcon = 5,
        ///ContactUs = 6,
        ///Service = 7,
        ///CompanyProfile = 8,
        ///CompanyOwnerIdentity = 9,
        ///CompanyCommercialRegister = 10,
        ///AdditionalAttachment = 11,
        ///SubService = 12,
        ///Tool = 13
        /// </summary>
        public byte RefType { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Accepted File Types: 1- Pdf, 2- Word, 3- Jpeg, 4- Png, 5- Jpg
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public IFormFile File { get; set; }

        /// <summary>
        /// AddValidationErrors
        /// </summary>
        /// <param name="context"></param>
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!Attachment.IsValidAttachmentRefType(RefType))
            {
                // context.Results.Add(new ValidationResult(L("InvalidAttachmentRefType"));
            }
        }
    }
}
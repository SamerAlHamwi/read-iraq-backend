using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Microsoft.AspNetCore.Http;
using ReadIraq.Domain.Attachments;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Attachments.Dto
{
    /// <summary>
    /// UploadMultiAttachmentInputDto
    /// </summary>
    [AutoMapTo(typeof(Attachment))]
    public class UploadMultiAttachmentInputDto : ICustomValidate
    {
        /// <summary>
        ///  Profile = 1,
        /// Advertisiment = 2,
        ///  QR = 3,
        /// Property = 4,
        ///  Category = 6,
        /// AttributeIcon = 7,
        /// ContactUs = 8,
        ///  City=9   
        /// 
        /// </summary>
        public byte RefType { get; set; }

        /// <summary>
        /// Accepted File Types: 1- Pdf, 2- Word, 3- Jpeg, 4- Png, 5- Jpg
        /// </summary>
        [Required(ErrorMessage = "Required")]
        public IFormFileCollection Files { get; set; }

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

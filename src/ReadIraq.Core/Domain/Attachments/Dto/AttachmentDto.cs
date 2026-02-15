using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Attachments;

namespace ReadIraq.Attachments.Dto
{
    /// <summary>
    /// AttachmentDto
    /// </summary>
    [AutoMapFrom(typeof(Attachment))]
    public class AttachmentDto : EntityDto
    {
        /// <summary>
        /// Post = 1
        /// </summary>
        public byte RefType { get; set; }


        /// <summary>
        /// Attachment Type:
        /// 1- Pdf,
        /// 2- Word,
        /// 3- Jpeg,
        /// 4- Png,
        /// 5- Jpg
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Attachment Url
        /// </summary>
        public string Url { get; set; }
        public string LowResolutionPhotosUrl { get; set; }

    }


}
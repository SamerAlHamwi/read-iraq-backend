using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.Attachments
{
    // Attachment model
    public class Attachment : FullAuditedEntity<long>
    {
        /// <summary>
        /// Original/display name of the file.
        /// </summary>
        [StringLength(500)]
        public string FileName { get; set; }

        /// <summary>
        /// Optional description for the media.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Media type (video/pdf/image/audio/other).
        /// </summary>
        public MediaType Type { get; set; }

        /// <summary>
        /// Storage key used to locate/delete the file in storage (local or external).
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string StorageKey { get; set; }

        /// <summary>
        /// Public URL to access the file.
        /// </summary>
        [StringLength(2000)]
        public string Url { get; set; }

        /// <summary>
        /// File size in bytes.
        /// </summary>
        public double Size { get; set; }

        public string LowResolutionPhotoRelativePath { get; set; }


        public string RefId { get; set; }

        public AttachmentRefType RefType { get; set; }
        public int? AttributeChoiceAndAttachmentId { get; set; }

        public static bool IsValidAttachmentRefType(byte type)
        {
            return Enum.IsDefined(typeof(AttachmentRefType), type);
        }
    }


}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Attachments;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Attachments.Dto
{
    /// <summary>
    /// AttachmentDto
    /// </summary>
    [AutoMapFrom(typeof(Attachment))]
    public class AttachmentDto : EntityDto<long>
    {
        public string FileName { get; set; }
        public string Description { get; set; }
        public MediaType Type { get; set; }
        public string Url { get; set; }
        public string StorageKey { get; set; }
        public double Size { get; set; }
        public DateTime CreatedAt { get; set; }

        // Backward compatible (not part of core model)
        public string LowResolutionPhotosUrl { get; set; }

    }


}
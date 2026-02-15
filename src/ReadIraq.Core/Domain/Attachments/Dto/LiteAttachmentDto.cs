using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Attachments;
using System.Collections.Generic;

namespace ReadIraq
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Attachment))]
    public class LiteAttachmentDto : EntityDto<long>
    {
        public string Url { get; set; }
        public string LowResolutionPhotoUrl { get; set; }
    }

    public class EnumInfo
    {
        public string Name { get; set; }
        public List<EnumValue> Values { get; set; }
    }

    public class EnumValue
    {
        public string Name { get; set; }
        public byte Value { get; set; }
    }
}

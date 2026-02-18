using Abp.Application.Services.Dto;

namespace ReadIraq.Attachments.Dto
{
    public class PagedAttachmentResultRequestDto : PagedResultRequestDto
    {
        public string RefId { get; set; }
        public byte RefType { get; set; }
    }
}

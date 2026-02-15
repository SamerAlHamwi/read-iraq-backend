using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.SubServices.Dto
{
    public class PagedSubServiceResultRequestDto : PagedResultRequestDto
    {
        public string KeyWord { get; set; }
        public int? ServiceId { get; set; }
        public int? ToolId { get; set; }

    }
}

using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.services.Dto
{
    public class PagedServiceResultRequestDto : PagedResultRequestDto
    {
        public string KeyWord { get; set; }
        public int? SubServiceId { get; set; }
        public int? ToolId { get; set; }
        public bool? IsForStorage { get; set; }
        public bool? IsForTruck { get; set; }
        public bool? Active { get; set; }

    }
}

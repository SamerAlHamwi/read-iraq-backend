using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Toolss.Dto
{
    public class PagedToolResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public int? ServiceId { get; set; }
        public int? SubServiceId { get; set; }
        public bool? IsActive { get; set; }

    }
}

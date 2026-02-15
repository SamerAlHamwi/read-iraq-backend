using Abp.Application.Services.Dto;

namespace ReadIraq.Mediators.Dto
{
    public class PagedMediatiorResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}

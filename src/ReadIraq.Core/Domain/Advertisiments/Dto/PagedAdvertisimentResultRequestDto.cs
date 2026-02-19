using Abp.Application.Services.Dto;

namespace ReadIraq.Advertisiments.Dto
{
    public class PagedAdvertisimentResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}

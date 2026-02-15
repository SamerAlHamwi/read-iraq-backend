using Abp.Application.Services.Dto;

namespace ReadIraq.Points.Dto
{
    public class PagedPointResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public bool IsForFeature { get; set; } = false;
    }
}

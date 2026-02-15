using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Partners.Dto
{
    public class PagedPartnerResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}

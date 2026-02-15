using Abp.Application.Services.Dto;

namespace ReadIraq.ContactUsService.Dto
{
    public class PagedContactUsResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

using Abp.Application.Services.Dto;

namespace ReadIraq.Domain.Drafts.Dtos
{
    public class PagedDraftResultRequestDto : PagedResultRequestDto
    {
        public long? UserId { get; set; }
    }

}

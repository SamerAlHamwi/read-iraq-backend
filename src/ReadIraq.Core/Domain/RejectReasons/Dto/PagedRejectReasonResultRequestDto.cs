using Abp.Application.Services.Dto;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.RejectReasons.Dto
{
    public class PagedRejectReasonResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public PossibilityPotentialClient? PossibilityPotentialClient { get; set; }

    }
}

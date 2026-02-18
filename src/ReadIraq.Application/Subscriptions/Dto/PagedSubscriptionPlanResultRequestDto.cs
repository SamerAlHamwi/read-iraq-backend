using Abp.Application.Services.Dto;

namespace ReadIraq.Subscriptions.Dto
{
    public class PagedSubscriptionPlanResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Subscriptions.Dto
{
    public class PagedSubscriptionResultRequestDto : PagedAndSortedResultRequestDto
    {
        public long? UserId { get; set; }
        public Guid? PlanId { get; set; }
        public bool? IsActive { get; set; }
    }
}

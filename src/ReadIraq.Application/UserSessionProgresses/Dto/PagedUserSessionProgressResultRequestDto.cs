using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.UserSessionProgresses.Dto
{
    public class PagedUserSessionProgressResultRequestDto : PagedAndSortedResultRequestDto
    {
        public long? UserId { get; set; }
        public Guid? SessionId { get; set; }
    }
}

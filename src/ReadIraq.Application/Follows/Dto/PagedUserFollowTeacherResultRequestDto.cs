using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Follows.Dto
{
    public class PagedUserFollowTeacherResultRequestDto : PagedAndSortedResultRequestDto
    {
        public long? UserId { get; set; }
        public Guid? TeacherProfileId { get; set; }
    }
}


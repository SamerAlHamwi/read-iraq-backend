using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Comments.Dto
{
    public class PagedSessionCommentResultRequestDto : PagedAndSortedResultRequestDto
    {
        public Guid? LessonSessionId { get; set; }
        public string Keyword { get; set; }
    }
}

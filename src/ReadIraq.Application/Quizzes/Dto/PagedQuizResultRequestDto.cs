using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Quizzes.Dto
{
    public class PagedQuizResultRequestDto : PagedAndSortedResultRequestDto
    {
        public Guid? SubjectId { get; set; }
        public Guid? SessionId { get; set; }
    }
}

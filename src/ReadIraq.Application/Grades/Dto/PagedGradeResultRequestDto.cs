using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Grades.Dto
{
    public class PagedGradeResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public Guid? GradeGroupId { get; set; }
    }
}

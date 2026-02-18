using Abp.Application.Services.Dto;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Subjects.Dto
{
    public class PagedSubjectResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public SubjectLevel? Level { get; set; }
        public int? GradeId { get; set; }
        public bool? IsActive { get; set; }
    }
}

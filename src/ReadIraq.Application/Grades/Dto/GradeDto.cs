using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Grades.Dto
{
    [AutoMapFrom(typeof(Grade))]
    public class GradeDto : EntityDto<int>
    {
        public List<TranslationDto> Name { get; set; }
        public int Priority { get; set; }
        public Guid GradeGroupId { get; set; }
        public List<Guid> SubjectIds { get; set; }
    }
}

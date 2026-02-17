using Abp.Application.Services.Dto;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.Grades.Dto
{
    public class GradeGroupDto : EntityDto<Guid>
    {
        public List<TranslationDto> Name { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

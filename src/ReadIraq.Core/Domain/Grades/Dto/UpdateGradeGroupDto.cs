using Abp.Application.Services.Dto;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Domain.Grades.Dto
{
    public class UpdateGradeGroupDto : EntityDto<Guid>
    {
        public List<CreateTranslationDto> Name { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
    }
}

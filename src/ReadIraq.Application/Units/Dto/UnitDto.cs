using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Units;
using ReadIraq.Domain.Translations.Dto;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Units.Dto
{
    [AutoMapFrom(typeof(Unit))]
    public class UnitDto : EntityDto<Guid>
    {
        public List<TranslationDto> Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public List<LiteLessonSessionDto> Lessons { get; set; }

        public UnitDto()
        {
            Lessons = new List<LiteLessonSessionDto>();
        }
    }
}

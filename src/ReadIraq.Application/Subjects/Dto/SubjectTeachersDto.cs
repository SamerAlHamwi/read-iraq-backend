using ReadIraq.Domain.Translations.Dto;
using ReadIraq.Teachers.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Subjects.Dto
{
    public class SubjectTeachersDto
    {
        public Guid SubjectId { get; set; }
        public List<TranslationDto> Name { get; set; }
        public List<LiteTeacherProfileDto> Teachers { get; set; }
    }
}

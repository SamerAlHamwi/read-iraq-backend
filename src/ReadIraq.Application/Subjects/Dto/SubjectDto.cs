using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Translations.Dto;
using ReadIraq.Teachers.Dto;
using ReadIraq.LessonSessions.Dto;
using System;
using System.Collections.Generic;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Subjects.Dto
{
    [AutoMapFrom(typeof(Subject))]
    public class SubjectDto : EntityDto<Guid>
    {
        public List<TranslationDto> Name { get; set; }
        public string Description { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public SubjectLevel Level { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int LessonsCount { get; set; }
        public string Color { get; set; }
        public double ProgressPercentage { get; set; }
        public LiteTeacherProfileDto TopTeacher { get; set; }
        public List<LiteLessonSessionDto> Lessons { get; set; }
    }
}

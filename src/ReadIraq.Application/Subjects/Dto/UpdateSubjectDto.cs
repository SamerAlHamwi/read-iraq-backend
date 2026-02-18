using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Subjects;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Subjects.Dto
{
    [AutoMapTo(typeof(Subject))]
    public class UpdateSubjectDto : EntityDto<Guid>
    {
        [Required]
        public List<TranslationDto> Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public SubjectLevel Level { get; set; }

        public List<int> GradeIds { get; set; }
    }
}

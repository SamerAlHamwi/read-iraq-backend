using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Grades;
using ReadIraq.Domain.Translations.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Grades.Dto
{
    [AutoMapTo(typeof(Grade))]
    public class UpdateGradeDto : EntityDto<int>
    {
        [Required]
        public List<TranslationDto> Name { get; set; }

        public int Priority { get; set; }

        public Guid GradeGroupId { get; set; }

        public List<Guid> SubjectIds { get; set; }
    }
}

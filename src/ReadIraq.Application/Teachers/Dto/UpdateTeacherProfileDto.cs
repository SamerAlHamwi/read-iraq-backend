using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapTo(typeof(TeacherProfile))]
    public class UpdateTeacherProfileDto : EntityDto<Guid>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public string Bio { get; set; }

        public string Specialization { get; set; }

        public long AttachmentId { get; set; }

        [StringLength(7)]
        public string Color { get; set; }

        public List<Guid> FeatureIds { get; set; }
        public List<TeacherSubjectDto> Subjects { get; set; }
    }
}

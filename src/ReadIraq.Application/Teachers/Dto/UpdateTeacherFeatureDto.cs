using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapTo(typeof(TeacherFeature))]
    public class UpdateTeacherFeatureDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}

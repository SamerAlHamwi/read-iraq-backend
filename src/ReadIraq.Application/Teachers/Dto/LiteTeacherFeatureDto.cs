using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherFeature))]
    public class LiteTeacherFeatureDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}

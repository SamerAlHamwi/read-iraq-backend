using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherRatingBreakdown))]
    public class TeacherRatingBreakdownDto : EntityDto<Guid>
    {
        public int Rating { get; set; }
        public int Count { get; set; }
    }
}

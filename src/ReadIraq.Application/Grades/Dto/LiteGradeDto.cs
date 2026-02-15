using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Grades;

namespace ReadIraq.Grades.Dto
{
    [AutoMapFrom(typeof(Grade))]
    public class LiteGradeDto : EntityDto<int>
    {
        public string Name { get; set; }
    }
}

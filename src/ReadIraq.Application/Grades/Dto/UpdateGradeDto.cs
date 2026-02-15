using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Grades;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Grades.Dto
{
    [AutoMapTo(typeof(Grade))]
    public class UpdateGradeDto : EntityDto<int>
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        public int Priority { get; set; }
    }
}

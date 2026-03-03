using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Codes;
using System;

namespace ReadIraq.ActivationCodes.Dto
{
    [AutoMapFrom(typeof(ActivationCode))]
    public class ActivationCodeDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public Guid? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Guid? TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int? GradeId { get; set; }
        public string GradeName { get; set; }
        public decimal Price { get; set; }
        public bool IsUsed { get; set; }
        public string UserName { get; set; }
        public DateTime? UsedDate { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

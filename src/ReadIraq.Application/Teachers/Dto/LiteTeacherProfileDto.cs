using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherProfile))]
    public class LiteTeacherProfileDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Specialization { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public decimal AverageRating { get; set; }
        public int LessonsCount { get; set; }
        public bool IsFollowed { get; set; }
        public bool IsSaved { get; set; }
    }
}

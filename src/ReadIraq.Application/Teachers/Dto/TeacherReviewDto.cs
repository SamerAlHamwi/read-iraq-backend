using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherReview))]
    public class TeacherReviewDto : EntityDto<Guid>
    {
        public Guid TeacherProfileId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } // Extra for UI
        public string UserAvatar { get; set; } // Extra for UI
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

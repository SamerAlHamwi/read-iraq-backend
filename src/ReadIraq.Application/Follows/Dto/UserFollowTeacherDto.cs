using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Follows;
using System;

namespace ReadIraq.Follows.Dto
{
    [AutoMapFrom(typeof(UserFollowTeacher))]
    public class UserFollowTeacherDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid TeacherProfileId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}


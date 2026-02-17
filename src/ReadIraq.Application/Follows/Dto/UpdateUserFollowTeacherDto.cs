using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Follows;
using System;

namespace ReadIraq.Follows.Dto
{
    [AutoMapTo(typeof(UserFollowTeacher))]
    public class UpdateUserFollowTeacherDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid TeacherProfileId { get; set; }
    }
}


using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace ReadIraq.Comments.Dto
{
    public class SessionCommentDto : EntityDto<Guid>
    {
        public Guid LessonSessionId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfilePicture { get; set; }
        public string Text { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsByTeacher { get; set; }
        public DateTime CreationTime { get; set; }
        public List<SessionCommentDto> Replies { get; set; }

        public SessionCommentDto()
        {
            Replies = new List<SessionCommentDto>();
        }
    }
}

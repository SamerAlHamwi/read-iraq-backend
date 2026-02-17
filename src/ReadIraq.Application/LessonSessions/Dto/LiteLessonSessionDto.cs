using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.LessonSessions;
using System;

namespace ReadIraq.LessonSessions.Dto
{
    [AutoMapFrom(typeof(LessonSession))]
    public class LiteLessonSessionDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }
        public Guid TeacherProfileId { get; set; }
        public int DurationSeconds { get; set; }
        public int Order { get; set; }
        public bool IsSavedByDefault { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
    }
}


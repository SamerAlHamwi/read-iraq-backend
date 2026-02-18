using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.LessonSessions;
using System;
using System.Collections.Generic;

namespace ReadIraq.LessonSessions.Dto
{
    [AutoMapFrom(typeof(LessonSession))]
    public class LessonSessionDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string VideoUrl { get; set; }
        public Guid TeacherProfileId { get; set; }
        public Guid SubjectId { get; set; }
        public int DurationSeconds { get; set; }
        public int Order { get; set; }
        public bool IsSavedByDefault { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsFree { get; set; }
        public List<long> AttachmentIds { get; set; }

        // Progress fields for current user
        public bool IsCompleted { get; set; }
        public int WatchedSeconds { get; set; }
    }
}

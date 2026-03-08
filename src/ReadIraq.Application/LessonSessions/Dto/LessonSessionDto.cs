using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.LessonSessions;
using System;
using System.Collections.Generic;
using ReadIraq;

namespace ReadIraq.LessonSessions.Dto
{
    [AutoMapFrom(typeof(LessonSession))]
    public class LessonSessionDto : EntityDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public LiteAttachmentDto Thumbnail { get; set; }
        public LiteAttachmentDto Video { get; set; }
        public Guid TeacherProfileId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherBio { get; set; }
        public string TeacherSpecialization { get; set; }
        public string TeacherImageUrl { get; set; }
        public decimal TeacherRating { get; set; }
        public int TeacherReviewsCount { get; set; }

        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; }

        public Guid? UnitId { get; set; }
        public string UnitName { get; set; }

        public int DurationSeconds { get; set; }
        public int Order { get; set; }
        public bool IsSavedByDefault { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsFree { get; set; }
        public List<LiteAttachmentDto> Attachments { get; set; }

        // Progress fields for current user
        public bool IsCompleted { get; set; }
        public bool CanTakeQuiz { get; set; }
        public int WatchedSeconds { get; set; }
        public bool IsSaved { get; set; }
        public bool IsEnrolled { get; set; }

        // Quiz Result fields
        public bool HasCompletedQuiz { get; set; }
        public int? QuizScore { get; set; }
        public int? QuizMaxScore { get; set; }
    }
}

using Abp.AutoMapper;
using ReadIraq.Domain.LessonSessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.LessonSessions.Dto
{
    [AutoMapTo(typeof(LessonSession))]
    public class CreateLessonSessionDto
    {
        [Required]
        [StringLength(500)]
        public string Title { get; set; }

        public string Description { get; set; }

        [StringLength(2000)]
        public string ThumbnailUrl { get; set; }

        [StringLength(2000)]
        public string VideoUrl { get; set; }

        public Guid TeacherProfileId { get; set; }

        public int DurationSeconds { get; set; }

        public int Order { get; set; }

        public bool IsSavedByDefault { get; set; }

        public List<long> AttachmentIds { get; set; } = new List<long>();
    }
}


using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;
using System.Collections.Generic;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapFrom(typeof(TeacherProfile))]
    public class TeacherProfileDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Specialization { get; set; }
        public LiteAttachmentDto Attachment { get; set; }
        public string Color { get; set; }
        public int StudentsCount { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public int LessonsCount { get; set; }
        public bool IsFollowed { get; set; }

        public List<Guid> FeatureIds { get; set; }
        public List<TeacherFeatureDto> Features { get; set; }
        public List<Guid> SubjectIds { get; set; }
        public List<TeacherRatingBreakdownDto> RatingBreakdowns { get; set; }
    }
}

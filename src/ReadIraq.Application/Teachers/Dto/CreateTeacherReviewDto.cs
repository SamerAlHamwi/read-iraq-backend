using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapTo(typeof(TeacherReview))]
    public class CreateTeacherReviewDto
    {
        [Required]
        public Guid TeacherProfileId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string ReviewText { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Teachers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Teachers.Dto
{
    [AutoMapTo(typeof(TeacherReview))]
    public class UpdateTeacherReviewDto : EntityDto<Guid>
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string ReviewText { get; set; }
    }
}

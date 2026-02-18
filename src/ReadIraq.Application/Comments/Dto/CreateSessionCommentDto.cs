using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Comments.Dto
{
    public class CreateSessionCommentDto
    {
        [Required]
        public Guid LessonSessionId { get; set; }

        [Required]
        public string Text { get; set; }

        public Guid? ParentCommentId { get; set; }
    }
}

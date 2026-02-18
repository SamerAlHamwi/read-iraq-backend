using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Comments.Dto
{
    public class UpdateSessionCommentDto : EntityDto<Guid>
    {
        [Required]
        public string Text { get; set; }
    }
}

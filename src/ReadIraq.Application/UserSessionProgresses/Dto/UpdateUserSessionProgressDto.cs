using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.UserSessionProgresses;
using System;
using System.ComponentModel.DataAnnotations;

namespace ReadIraq.UserSessionProgresses.Dto
{
    [AutoMapTo(typeof(UserSessionProgress))]
    public class UpdateUserSessionProgressDto : EntityDto<Guid>
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public Guid SessionId { get; set; }

        public int WatchedSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime LastWatchedAt { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.UserSessionProgresses;
using System;

namespace ReadIraq.UserSessionProgresses.Dto
{
    [AutoMapFrom(typeof(UserSessionProgress))]
    public class UserSessionProgressDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public Guid SessionId { get; set; }
        public int WatchedSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastWatchedAt { get; set; }
    }
}

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.Audit;
using System;

namespace ReadIraq.Audit.Dto
{
    [AutoMapFrom(typeof(ActivityLog))]
    public class ActivityLogDto : EntityDto<Guid>
    {
        public long ActorId { get; set; }
        public string ActionType { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public string Metadata { get; set; }
        public DateTime CreationTime { get; set; }
    }

    [AutoMapTo(typeof(ActivityLog))]
    public class CreateActivityLogDto
    {
        public long ActorId { get; set; }
        public string ActionType { get; set; }
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public string Metadata { get; set; }
    }
}

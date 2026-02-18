using Abp.Application.Services.Dto;
using System;

namespace ReadIraq.Gifts.Dto
{
    public class GiftDto : EntityDto<Guid>
    {
        public long TargetUserId { get; set; }
        public string TargetUserName { get; set; }
        public Guid? PlanId { get; set; }
        public string PlanName { get; set; }
        public string Note { get; set; }
        public long AdminUserId { get; set; }
        public string AdminUserName { get; set; }
        public DateTime CreationTime { get; set; }
    }
}

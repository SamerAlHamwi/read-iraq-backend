using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.SavedItems;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.SavedItems.Dto
{
    [AutoMapFrom(typeof(UserSavedItem))]
    public class LiteUserSavedItemDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public SavedItemType ItemType { get; set; }
        public Guid ItemId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}


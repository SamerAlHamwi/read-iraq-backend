using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ReadIraq.Domain.SavedItems;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.SavedItems.Dto
{
    [AutoMapTo(typeof(UserSavedItem))]
    public class UpdateUserSavedItemDto : EntityDto<Guid>
    {
        public long UserId { get; set; }
        public SavedItemType ItemType { get; set; }
        public Guid ItemId { get; set; }
    }
}


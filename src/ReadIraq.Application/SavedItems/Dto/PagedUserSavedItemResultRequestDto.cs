using Abp.Application.Services.Dto;
using System;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.SavedItems.Dto
{
    public class PagedUserSavedItemResultRequestDto : PagedAndSortedResultRequestDto
    {
        public long? UserId { get; set; }
        public SavedItemType? ItemType { get; set; }
        public Guid? ItemId { get; set; }
    }
}


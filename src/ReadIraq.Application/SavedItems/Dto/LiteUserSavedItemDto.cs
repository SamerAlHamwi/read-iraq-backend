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

        // UI Helpers
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string DurationText { get; set; }
        public string SubjectName { get; set; }
        public string TeacherName { get; set; }
        public string TeacherImageUrl { get; set; }
        public string Department { get; set; }
        public double? Rating { get; set; }
    }
}

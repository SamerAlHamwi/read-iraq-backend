using Abp.Domain.Entities.Auditing;
using ReadIraq.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static ReadIraq.Enums.Enum;

namespace ReadIraq.Domain.SavedItems
{
    [Table("UserSavedItems")]
    public class UserSavedItem : FullAuditedEntity<Guid>
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public SavedItemType ItemType { get; set; }

        public Guid ItemId { get; set; }
    }
}
